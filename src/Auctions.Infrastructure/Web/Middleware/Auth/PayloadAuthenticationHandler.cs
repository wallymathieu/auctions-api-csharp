using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

/// <summary>
///     See also https://github.com/MaximRouiller/MaximeRouiller.Azure.AppService.EasyAuth
/// </summary>
internal class PayloadAuthenticationHandler : AuthenticationHandler<PayloadAuthenticationOptions>
{
    private readonly ClaimsPrincipalParser _claimsPrincipalParser;
    private readonly JwtPayloadClaimsPrincipalParser _jwtPayloadClaimsPrincipalParser;

    public PayloadAuthenticationHandler(IOptionsMonitor<PayloadAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ClaimsPrincipalParser claimsPrincipalParser,
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
        catch (Exception e)
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }
    }
}