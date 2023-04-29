using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Wallymathieu.Auctions.Api.Middleware.Auth;

public class DecodedHeaderAuthorizationFilter:IAuthorizationFilter
{
    private readonly string _principalHeaderName ;

    private readonly IClaimsPrincipalParser _claimsPrincipalParser;

    public DecodedHeaderAuthorizationFilter(IClaimsPrincipalParser claimsPrincipalParser, IConfiguration configuration)
    {
        _claimsPrincipalParser = claimsPrincipalParser;
        _principalHeaderName = configuration["PrincipalHeader"] ?? "x-jwt-payload";
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.Request.Headers[_principalHeaderName];

        if (!_claimsPrincipalParser.IsValid(apiKey, out var identify))
        {
            context.Result = new UnauthorizedResult();
        }
        else
        {
            context.HttpContext.User = identify;
        }
    }
}