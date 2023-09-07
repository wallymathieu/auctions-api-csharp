using System.Security.Claims;

namespace Wallymathieu.Auctions.Api.Middleware.Auth;

public interface IClaimsPrincipalParser
{
    bool IsValid(string? apiKey, out ClaimsPrincipal? claimsIdentity);
}