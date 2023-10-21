using Microsoft.AspNetCore.Authentication;

namespace Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

public class PayloadAuthenticationOptions : AuthenticationSchemeOptions
{
    public string PrincipalHeader { get; set; } = JwtPayload.Header;
}