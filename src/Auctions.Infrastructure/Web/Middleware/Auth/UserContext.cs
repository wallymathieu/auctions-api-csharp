using Microsoft.AspNetCore.Http;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

internal class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserId UserId => new(_httpContextAccessor.HttpContext?.User?.Identity?.Name ??
                                throw new Exception("Missing user identity"));
}