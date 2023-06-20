using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Wallymathieu.Auctions.Api.Middleware.Auth;
/// <summary>
/// See also https://github.com/MaximRouiller/MaximeRouiller.Azure.AppService.EasyAuth
/// </summary>
public class PayloadAuthenticationHandler: AuthenticationHandler<PayloadAuthenticationOptions>
{
    private readonly IClaimsPrincipalParser _claimsPrincipalParser;
    public PayloadAuthenticationHandler(IOptionsMonitor<PayloadAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock, IClaimsPrincipalParser claimsPrincipalParser) : base(options, logger, encoder, clock)
    {
        _claimsPrincipalParser = claimsPrincipalParser;
    }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var apiKey = Request.Headers[Options.PrincipalHeader];
            if (string.IsNullOrEmpty(apiKey))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            if (_claimsPrincipalParser.IsValid(apiKey,out var claimsIdentity))
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