using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth.JwtPayloads;

internal sealed class JwtPayloadClaimsPrincipalParser : IClaimsPrincipalParser
{
    private readonly ILogger<JwtPayloadClaimsPrincipalParser> _logger;

    public JwtPayloadClaimsPrincipalParser(ILogger<JwtPayloadClaimsPrincipalParser> logger)
    {
        _logger = logger;
    }

    public bool IsValid(string? apiKey, out ClaimsPrincipal? claimsIdentity)
    {
        claimsIdentity = null;
        if (string.IsNullOrWhiteSpace(apiKey)) return false;
        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(apiKey));
            var deserialized = JsonSerializer.Deserialize<JwtPayload>(json);
            if (deserialized == null || string.IsNullOrEmpty(deserialized.Name)) return false;
            claimsIdentity = new ClaimsPrincipal(
            [
                new ClaimsIdentity(
                [
                    new(ClaimTypes.Name, deserialized.Name)
                ], "proxy", ClaimTypes.Name, ClaimTypes.Role)
            ]);
            return true;
        }
#pragma warning disable CA1031 // we want to catch all exceptions and pass error to logger
        catch (Exception e)
#pragma warning restore CA1031
        {
            _logger.LogWarning(e, "Failed to decode JWT body header");
            return false;
        }
    }

    internal sealed class JwtPayload
    {
        [JsonPropertyName("sub")] public string? Sub { get; set; }

        [JsonPropertyName("name")] public string? Name { get; set; }

        [JsonPropertyName("u_typ")] public string? UTyp { get; set; }
    }
}