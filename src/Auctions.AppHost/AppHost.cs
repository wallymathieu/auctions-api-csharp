using System.Text.Json;
using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("redis");
var sql = builder.AddSqlServer("db")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);
var db = sql.AddDatabase("auctions");

var migrationService = builder.AddProject<Projects.Auctions_MigrationService>("migration")
    .WithReference(db)
    .WaitFor(db);

var api = builder.AddProject<Projects.Auctions_WebApi>("api")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(db)
    .WaitFor(db)
    .WaitFor(migrationService)
    .WithHttpHealthCheck("/health");

var issuer = builder.AddParameter("jwt-issuer");
var audience = builder.AddParameter("jwt-audience");
var jwksUri = builder.AddParameter("jwt-jwks-uri");
var envoyConfig = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "envoy.yaml"));

builder.AddContainer("envoy",
        "envoyproxy/envoy:v1.39.2@sha256:460f8c329f24b2e1c7c5af64cb314a6f351ad1c262ba77b135ac46b35ebd5f85")
    .WithHttpEndpoint(targetPort: 10000)
    .WithExternalHttpEndpoints()
    .WithArgs("--config-path", "/etc/envoy/envoy.yaml")
    .WithContainerFiles("/etc/envoy", async (context, cancellationToken) =>
    {
        var upstream = await api.GetEndpoint("http").Property(EndpointProperty.Url)
            .GetValueAsync(new ValueProviderContext { Caller = context.Model }, cancellationToken);
        var configuredIssuer = await issuer.Resource.GetValueAsync(cancellationToken);
        var configuredAudience = await audience.Resource.GetValueAsync(cancellationToken);
        var configuredJwks = await jwksUri.Resource.GetValueAsync(cancellationToken);
        if (!Uri.TryCreate(upstream, UriKind.Absolute, out var upstreamUrl) ||
            !Uri.TryCreate(configuredIssuer, UriKind.Absolute, out var issuerUrl) ||
            issuerUrl.Scheme != Uri.UriSchemeHttps ||
            !Uri.TryCreate(configuredJwks, UriKind.Absolute, out var jwksUrl) ||
            jwksUrl.Scheme != Uri.UriSchemeHttps || jwksUrl.Port != 443 ||
            string.IsNullOrWhiteSpace(configuredAudience))
            throw new InvalidOperationException("Envoy requires an HTTPS issuer, audience, HTTPS JWKS URL on port 443, and API endpoint.");

        var yaml = envoyConfig
            .Replace("__ISSUER__", JsonSerializer.Serialize(configuredIssuer), StringComparison.Ordinal)
            .Replace("__AUDIENCE__", JsonSerializer.Serialize(configuredAudience), StringComparison.Ordinal)
            .Replace("__JWKS_URI__", JsonSerializer.Serialize(configuredJwks), StringComparison.Ordinal)
            .Replace("__JWKS_HOST__", JsonSerializer.Serialize(jwksUrl.Host), StringComparison.Ordinal)
            .Replace("__API_HOST__", JsonSerializer.Serialize(upstreamUrl.Host), StringComparison.Ordinal)
            .Replace("__API_PORT__", upstreamUrl.Port.ToString(System.Globalization.CultureInfo.InvariantCulture),
                StringComparison.Ordinal);
        return [new ContainerFile { Name = "envoy.yaml", Contents = yaml }];
    })
    .WithReference(api)
    .WaitFor(api);

builder.AddProject<Projects.Auctions_Frontend>("frontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(db)
    .WaitFor(db)
    .WaitFor(migrationService)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
