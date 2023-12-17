using System.Security.Claims;

namespace Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

public interface IClaimsPrincipalParser
{
    bool IsValid(string? apiKey, out ClaimsPrincipal? claimsIdentity);
}