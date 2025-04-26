using Microsoft.AspNetCore.Authentication;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth.JwtPayloads;

namespace Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

public class PayloadAuthenticationOptions : AuthenticationSchemeOptions
{
    public string PrincipalHeader { get; set; } = JwtPayloadClaimsPrincipal.Header;
}