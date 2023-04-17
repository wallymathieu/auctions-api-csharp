using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace App.Middleware.Auth;
/// <summary>
/// See https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-user-identities
/// </summary>
public class ClaimsPrincipalParser:IClaimsPrincipalParser{

    private class ClientPrincipalClaim
    {
        [JsonPropertyName("typ")]
        public string Type { get; set; }
        [JsonPropertyName("val")]
        public string Value { get; set; }
    }

    private class ClientPrincipal
    {
        [JsonPropertyName("auth_typ")]
        public string IdentityProvider { get; set; }
        [JsonPropertyName("name_typ")]
        public string NameClaimType { get; set; }
        [JsonPropertyName("role_typ")]
        public string RoleClaimType { get; set; }
        [JsonPropertyName("claims")]
        public IEnumerable<ClientPrincipalClaim> Claims { get; set; }
    }

    private readonly ILogger<ClaimsPrincipalParser> _logger;

    public ClaimsPrincipalParser(ILogger<ClaimsPrincipalParser> logger)
    {
        _logger = logger;
    }

    public bool IsValid(string? apiKey, out ClaimsPrincipal? claimsIdentity)
    {
        claimsIdentity = null;
        if (string.IsNullOrWhiteSpace(apiKey)) return false;
        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(apiKey));
            var principal = JsonSerializer.Deserialize<ClientPrincipal>(json);
            if (principal == null) return false;
            var identity = new ClaimsIdentity(principal.IdentityProvider,principal.NameClaimType,principal.RoleClaimType);
            identity.AddClaims(principal.Claims.Select(c => new Claim(c.Type, c.Value)));
        
            claimsIdentity = new ClaimsPrincipal(identity);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to decode body header"); 
            return false;
        }
    }
}