using Microsoft.AspNetCore.Authentication;

namespace Wallymathieu.Auctions.Api.Middleware.Auth;

public class PayloadAuthenticationOptions : AuthenticationSchemeOptions
{
    public string PrincipalHeader { get; set; } = JwtPayloadClaimsPrincipalParser.Header;
}