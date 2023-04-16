using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Tests;

public class ApiJwtDecodedValidator:IApiJwtDecodedValidator
{
    private readonly ILogger<ApiJwtDecodedValidator> _logger;

    public ApiJwtDecodedValidator(ILogger<ApiJwtDecodedValidator> logger)
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
            var deserialized = JsonConvert.DeserializeObject<JwtPayload>(json);
            if (deserialized == null) return false;
            claimsIdentity = new ClaimsPrincipal(new []
            {
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new(ClaimTypes.Name,deserialized.Name),
                    })
            });
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to decode JWT body header"); 
            return false;
        }
    }
}