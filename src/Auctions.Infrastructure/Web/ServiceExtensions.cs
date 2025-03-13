using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Web;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuctionsWebJwt(this IServiceCollection services)
    {
        return services
            .AddSingleton<ClaimsPrincipalParser>()
            .AddSingleton<JwtPayloadClaimsPrincipalParser>();
    }

    public static IServiceCollection AddHttpContextUserContext(this IServiceCollection services)
    {
        return services.AddScoped<IUserContext, UserContext>();
    }
}