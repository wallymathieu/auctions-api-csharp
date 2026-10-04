using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Web;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

namespace Wallymathieu.Auctions.Tests;

public class EnvoyProxyTests
{
    private const string Image =
        "envoyproxy/envoy:v1.39.2@sha256:460f8c329f24b2e1c7c5af64cb314a6f351ad1c262ba77b135ac46b35ebd5f85";
    private const string Issuer = "https://issuer.example/";
    private const string Audience = "auctions-api";

    [Fact]
    public async Task Envoy_verifies_tokens_and_cannot_be_spoofed()
    {
        var apiPort = FreePort();
        var jwksPort = FreePort();
        var proxyPort = FreePort();
        using var key = RSA.Create(2048);
        using var rotatedKey = RSA.Create(2048);
        using var certificateKey = RSA.Create(2048);
        var request = new CertificateRequest("CN=host.docker.internal", certificateKey,
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var names = new SubjectAlternativeNameBuilder();
        names.AddDnsName("host.docker.internal");
        request.CertificateExtensions.Add(names.Build());
        using var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(1));
        var currentKey = key;
        var keysAvailable = true;

        var apiBuilder = WebApplication.CreateBuilder();
        apiBuilder.WebHost.UseKestrel(options => options.ListenAnyIP(apiPort));
        apiBuilder.Services.AddAuctionsWebJwt();
        apiBuilder.Services.AddAuthentication().AddPayloadAuthentication();
        apiBuilder.Services.AddAuthorization();
        await using var api = apiBuilder.Build();
        api.UseAuthentication();
        api.UseAuthorization();
        api.MapGet("/auctions", (HttpContext context) => Results.Json(new
        {
            name = context.User.Identity?.Name,
            jwtHeader = context.Request.Headers["x-jwt-payload"].ToString(),
            azureHeader = context.Request.Headers["x-ms-client-principal"].ToString(),
            authorization = context.Request.Headers.Authorization.ToString()
        }));
        api.MapPost("/auctions", (HttpContext context) =>
            Results.Text(context.User.Identity?.Name ?? "")).RequireAuthorization();
        api.MapGet("/health", (HttpContext context) =>
            Results.Text(context.Request.Headers["x-jwt-payload"].ToString()));
        await api.StartAsync();

        var jwksBuilder = WebApplication.CreateBuilder();
        jwksBuilder.WebHost.UseKestrel(options =>
            options.ListenAnyIP(jwksPort, listen => listen.UseHttps(certificate)));
        await using var jwks = jwksBuilder.Build();
        jwks.MapGet("/jwks", () => keysAvailable
            ? Results.Json(new { keys = new[] { Jwk(currentKey) } })
            : Results.StatusCode(503));
        await jwks.StartAsync();

        using var directory = new TemporaryDirectory();
        var config = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "envoy.yaml"))
            .Replace("__ISSUER__", Issuer, StringComparison.Ordinal)
            .Replace("__AUDIENCE__", Audience, StringComparison.Ordinal)
            .Replace("__JWKS_URI__", $"https://host.docker.internal:{jwksPort}/jwks", StringComparison.Ordinal)
            .Replace("__JWKS_HOST__", "host.docker.internal", StringComparison.Ordinal)
            .Replace("__API_HOST__", "host.docker.internal", StringComparison.Ordinal)
            .Replace("__API_PORT__", apiPort.ToString(), StringComparison.Ordinal)
            .Replace("port_value: 443", $"port_value: {jwksPort}", StringComparison.Ordinal)
            .Replace("cache_duration: 300s", "cache_duration: 1s", StringComparison.Ordinal)
            .Replace("/etc/ssl/certs/ca-certificates.crt", "/etc/envoy/issuer.pem", StringComparison.Ordinal);
        File.WriteAllText(Path.Combine(directory.Path, "envoy.yaml"), config);
        File.WriteAllText(Path.Combine(directory.Path, "issuer.pem"),
            certificate.ExportCertificatePem());

        var id = await Docker("run", "-d", "--rm", "--add-host=host.docker.internal:host-gateway",
            "-p", $"127.0.0.1:{proxyPort}:10000",
            "-v", $"{directory.Path}/envoy.yaml:/etc/envoy/envoy.yaml:ro",
            "-v", $"{directory.Path}/issuer.pem:/etc/envoy/issuer.pem:ro", Image,
            "--config-path", "/etc/envoy/envoy.yaml");
        try
        {
            using var client = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{proxyPort}") };
            HttpResponseMessage response = null!;
            for (var attempt = 0; attempt < 40; attempt++)
            {
                try
                {
                    response = await client.GetAsync("/auctions");
                    break;
                }
                catch (HttpRequestException)
                {
                    await Task.Delay(250);
                }
            }
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using (var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
                Assert.Equal("", body.RootElement.GetProperty("jwtHeader").GetString());

            using (var missing = new HttpRequestMessage(HttpMethod.Post, "/auctions"))
                Assert.Equal(HttpStatusCode.Unauthorized, (await client.SendAsync(missing)).StatusCode);

            var valid = Token(key, Issuer, Audience, DateTimeOffset.UtcNow.AddMinutes(5), "Zoë 😀");
            using (var authorized = new HttpRequestMessage(HttpMethod.Post, "/auctions"))
            {
                authorized.Headers.Authorization = new AuthenticationHeaderValue("Bearer", valid);
                authorized.Headers.TryAddWithoutValidation("x-jwt-payload", new[] { "Zm9yZ2Vk", "YW5vdGhlcg" });
                authorized.Headers.TryAddWithoutValidation("x-ms-client-principal", "Zm9yZ2Vk");
                using var posted = await client.SendAsync(authorized);
                Assert.Equal(HttpStatusCode.OK, posted.StatusCode);
                Assert.Equal("Zoë 😀", await posted.Content.ReadAsStringAsync());
            }
            using (var signed = await Send(client, valid))
            {
                Assert.Equal(HttpStatusCode.OK, signed.StatusCode);
                using var payload = JsonDocument.Parse(await signed.Content.ReadAsStringAsync());
                Assert.Equal("Zoë 😀", payload.RootElement.GetProperty("name").GetString());
                Assert.Equal("", payload.RootElement.GetProperty("authorization").GetString());
                Assert.Equal("", payload.RootElement.GetProperty("azureHeader").GetString());
                var encoded = payload.RootElement.GetProperty("jwtHeader").GetString()!;
                Assert.DoesNotContain('.', encoded);
                Assert.Equal("Zoë 😀", JsonDocument.Parse(Encoding.UTF8.GetString(
                    Convert.FromBase64String(Padded(encoded)))).RootElement.GetProperty("name").GetString());
            }

            foreach (var invalid in new[]
                     {
                         Token(rotatedKey, Issuer, Audience, DateTimeOffset.UtcNow.AddMinutes(5), "forged"),
                         Token(key, "https://other.example/", Audience, DateTimeOffset.UtcNow.AddMinutes(5), "forged"),
                         Token(key, Issuer, "wrong", DateTimeOffset.UtcNow.AddMinutes(5), "forged"),
                         Token(key, Issuer, Audience, DateTimeOffset.UtcNow.AddMinutes(-5), "forged"),
                         Token(key, Issuer, Audience, null, "forged")
                     })
            {
                using var rejected = await Send(client, invalid);
                Assert.True(rejected.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden);
            }

            using (var forged = new HttpRequestMessage(HttpMethod.Post, "/auctions"))
            {
                forged.Headers.TryAddWithoutValidation("x-jwt-payload", "Zm9yZ2Vk");
                forged.Headers.TryAddWithoutValidation("x-ms-client-principal", "Zm9yZ2Vk");
                Assert.Equal(HttpStatusCode.Unauthorized, (await client.SendAsync(forged)).StatusCode);
            }
            using (var forged = new HttpRequestMessage(HttpMethod.Get, "/auctions"))
            {
                forged.Headers.TryAddWithoutValidation("x-jwt-payload", new[] { "Zm9yZ2Vk", "YW5vdGhlcg" });
                forged.Headers.TryAddWithoutValidation("x-ms-client-principal", "Zm9yZ2Vk");
                using var sanitized = await client.SendAsync(forged);
                Assert.Equal(HttpStatusCode.OK, sanitized.StatusCode);
                using var payload = JsonDocument.Parse(await sanitized.Content.ReadAsStringAsync());
                Assert.Equal("", payload.RootElement.GetProperty("jwtHeader").GetString());
                Assert.Equal("", payload.RootElement.GetProperty("azureHeader").GetString());
            }
            using (var health = new HttpRequestMessage(HttpMethod.Get, "/health"))
            {
                health.Headers.TryAddWithoutValidation("x-jwt-payload", "Zm9yZ2Vk");
                using var sanitized = await client.SendAsync(health);
                Assert.Equal(HttpStatusCode.OK, sanitized.StatusCode);
                Assert.Equal("", await sanitized.Content.ReadAsStringAsync());
            }

            currentKey = rotatedKey;
            await Task.Delay(1200);
            using (var rotated = await Send(client,
                       Token(rotatedKey, Issuer, Audience, DateTimeOffset.UtcNow.AddMinutes(5), "rotated")))
                Assert.Equal(HttpStatusCode.OK, rotated.StatusCode);
            keysAvailable = false;
            await Task.Delay(1200);
            using (var unavailable = await Send(client,
                       Token(key, Issuer, Audience, DateTimeOffset.UtcNow.AddMinutes(5), "forged")))
                Assert.True(unavailable.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden);
        }
        finally
        {
            await Docker("rm", "-f", id.Trim());
        }
    }

    private static async Task<HttpResponseMessage> Send(HttpClient client, string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/auctions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(request);
    }

    private static object Jwk(RSA rsa)
    {
        var parameters = rsa.ExportParameters(false);
        return new { kty = "RSA", kid = "test", alg = "RS256", use = "sig",
            n = Url(parameters.Modulus!), e = Url(parameters.Exponent!) };
    }

    private static string Token(RSA rsa, string issuer, string audience, DateTimeOffset? expiration, string name)
    {
        var header = Url(JsonSerializer.SerializeToUtf8Bytes(new { alg = "RS256", typ = "JWT", kid = "test" }));
        var body = Url(JsonSerializer.SerializeToUtf8Bytes(new
        {
            iss = issuer, aud = audience, exp = expiration?.ToUnixTimeSeconds(), sub = "different-id", name
        }, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
        var signingInput = $"{header}.{body}";
        return $"{signingInput}.{Url(rsa.SignData(Encoding.ASCII.GetBytes(signingInput),
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))}";
    }

    private static string Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static string Padded(string value) =>
        value.Replace('-', '+').Replace('_', '/').PadRight((value.Length + 3) / 4 * 4, '=');

    private static int FreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static async Task<string> Docker(params string[] args)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo("docker")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        foreach (var arg in args) process.StartInfo.ArgumentList.Add(arg);
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        Assert.True(process.ExitCode == 0, $"docker {args[0]} failed: {error}");
        return output.Trim();
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        public string Path { get; } = Directory.CreateTempSubdirectory("auctions-envoy-").FullName;
        public void Dispose() => Directory.Delete(Path, true);
    }
}
