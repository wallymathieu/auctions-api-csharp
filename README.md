# Auction API written in C\#

There are currently these main implementations:

- [main](https://github.com/wallymathieu/auctions-api-csharp/tree/main) is the core simple implementation
- [application-layer](https://github.com/wallymathieu/auctions-api-csharp/tree/application-layer) is the implementation with an application layer
- [command-handlers-infrastructure](https://github.com/wallymathieu/auctions-api-csharp/tree/command-handlers-infrastructure) is the implementation but without hand written command handlers that binds to the entity methods
- [command-handlers-mediatr](https://github.com/wallymathieu/auctions-api-csharp/tree/command-handlers-mediatr) is an extension of the command-handlers-infrastructure but with MediatR pipeline behavior instead of decorators

## Getting started

To build the apps run:

```bash
dotnet watch run --project ./src/Auctions.AppHost
```

## Auth and ingress

The Aspire host runs standalone Envoy in front of the API. Supply these required
Aspire parameters (through user secrets or environment variables) before starting:

| Parameter | Environment variable | Meaning |
| --- | --- | --- |
| `jwt-issuer` | `Parameters__jwt-issuer` | Exact HTTPS JWT `iss` value |
| `jwt-audience` | `Parameters__jwt-audience` | Expected API `aud` |
| `jwt-jwks-uri` | `Parameters__jwt-jwks-uri` | HTTPS JWKS URL on port 443 |

For example, set those three environment variables to your issuer's values,
then run `dotnet watch run --project ./src/Auctions.AppHost` and use the Envoy
endpoint shown in the Aspire dashboard. Send a signed JWT in the HTTP
Authorization header with the bearer-token scheme.
The issuer must include a nonempty `name` claim: **`name`, not `sub`, is the
application user ID**. Envoy checks issuer, audience, signature and expiration
against the issuer's JWKS, removes the bearer token and forwards only verified
Base64URL claims in `x-jwt-payload`. Missing tokens can read auctions; invalid
tokens are rejected, and creating auctions or bids requires authentication.
Clients must never supply claims headers as credentials.

`deploy/envoy/envoy.yaml` is the configuration template used by Aspire. For
another hosting target, replace its `__...__` placeholders with properly
YAML-escaped deployment values, retain the pinned Envoy image and HTTPS JWKS
certificate/hostname verification, and publish only Envoy. Configure the API
as a private upstream with ingress/firewall rules allowing **only Envoy** to
connect to it; removing Aspire's external endpoint flag does not by itself
prevent local direct access or enforce production network isolation. The
hosting target and its network/ingress controls must be specified and a direct
external connection to the API must be tested as blocked before deployment.
The frontend is a separate external application and is not an API security
boundary.

If TLS terminates before the API, configure `ReverseProxy__KnownNetworks__0`
to the trusted Envoy network CIDR (and additional numbered entries if needed)
so ASP.NET Core accepts forwarded scheme information before HTTPS redirection.
Do not trust arbitrary client IP ranges. The address must match the actual
network used by the chosen hosting target; do not assume the Aspire development
network is a production trust boundary. Local tests that call the API directly
with claims headers are not tests of this proxy boundary.

For legacy deployments using Azure's encoded claims principal, `PrincipalHeader`
can still be set to `x-ms-client-principal`, but do not expose that backend
directly to untrusted clients.

## Add migration

```bash
dotnet ef migrations add NewMigration --project ./src/Auctions.Infrastructure/Auctions.Infrastructure.csproj --startup-project ./src/Auctions.WebApi/Auctions.WebApi.csproj
```

## Inspiration

The main inspiration for the architecture of the API is found in this book:

- [Clean Architecture](https://www.goodreads.com/en/book/show/18043011)

Note that there are many variants of "the clean architecture" described in the .net space with different interpretations of what it means to implement this architecture.
