using System.Security.Claims;

namespace App.Middleware.Auth;

public interface IClaimsPrincipalParser
{
    bool IsValid(string? apiKey, out ClaimsPrincipal? claimsIdentity);
}