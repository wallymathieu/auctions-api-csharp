using System.Security.Claims;

namespace Tests;

public interface IApiJwtDecodedValidator
{
    bool IsValid(string? apiKey, out ClaimsPrincipal? claimsIdentity);
}