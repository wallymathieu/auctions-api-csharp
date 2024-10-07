using Microsoft.AspNetCore.Http;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserId UserId => new(_httpContextAccessor.HttpContext?.User?.Identity?.Name ??
                                throw new InvalidOperationException("Missing user identity"));
}