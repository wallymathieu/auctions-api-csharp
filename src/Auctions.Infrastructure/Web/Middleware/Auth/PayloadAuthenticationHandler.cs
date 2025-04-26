using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth.AzureClaimsPrincipals;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth.JwtPayloads;

namespace Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

internal sealed class PayloadAuthenticationHandler : AuthenticationHandler<PayloadAuthenticationOptions>
{
    private readonly AzureDecodedClaimsPrincipalParser _claimsPrincipalParser;
    private readonly JwtPayloadClaimsPrincipalParser _jwtPayloadClaimsPrincipalParser;

    public PayloadAuthenticationHandler(IOptionsMonitor<PayloadAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        AzureDecodedClaimsPrincipalParser claimsPrincipalParser,
        JwtPayloadClaimsPrincipalParser jwtPayloadClaimsPrincipalParser) : base(options, logger, encoder)
    {
        _claimsPrincipalParser = claimsPrincipalParser;
        _jwtPayloadClaimsPrincipalParser = jwtPayloadClaimsPrincipalParser;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var apiKey = Request.Headers[Options.PrincipalHeader];
            if (string.IsNullOrEmpty(apiKey)) return Task.FromResult(AuthenticateResult.NoResult());

            IClaimsPrincipalParser parser = Options.PrincipalHeader == JwtPayloadClaimsPrincipal.Header
                ? _jwtPayloadClaimsPrincipalParser
                : _claimsPrincipalParser;
            if (parser.IsValid(apiKey, out var claimsIdentity))
            {
                Context.User = claimsIdentity!;
                var success = AuthenticateResult.Success(
                    new AuthenticationTicket(claimsIdentity!, PayloadAuthenticationDefaults.AuthenticationScheme));

                return Task.FromResult(success);
            }

            return Task.FromResult(AuthenticateResult.Fail("InvalidClaimsIdentity"));
        }
#pragma warning disable CA1031 // we want to catch all exceptions and return a fail result
        catch (Exception e)
#pragma warning restore CA1031
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }
    }
}