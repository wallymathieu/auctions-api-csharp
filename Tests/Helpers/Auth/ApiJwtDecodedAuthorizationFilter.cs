using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Tests;

public class ApiJwtDecodedAuthorizationFilter:IAuthorizationFilter
{
    private const string ApiKeyHeaderName = "x-jwt-payload";

    private readonly IApiJwtDecodedValidator _apiJwtDecodedValidator;

    public ApiJwtDecodedAuthorizationFilter(IApiJwtDecodedValidator apiJwtDecodedValidator)
    {
        _apiJwtDecodedValidator = apiJwtDecodedValidator;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];

        if (!_apiJwtDecodedValidator.IsValid(apiKey, out var identify))
        {
            context.Result = new UnauthorizedResult();
        }
        else
        {
            context.HttpContext.User = identify;
        }
    }
}