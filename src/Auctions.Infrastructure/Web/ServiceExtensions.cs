using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Web;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuctionsWebJwt(this IServiceCollection services) =>
        services
            .AddSingleton<ClaimsPrincipalParser>()
            .AddSingleton<JwtPayloadClaimsPrincipalParser>();

    public static IServiceCollection AddHttpContextUserContext(this IServiceCollection services) =>
        services.AddScoped<IUserContext, UserContext>();
}
