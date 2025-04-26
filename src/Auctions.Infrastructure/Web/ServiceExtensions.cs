using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth.AzureClaimsPrincipals;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth.JwtPayloads;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Web;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuctionsWebJwt(this IServiceCollection services) =>
        services
            .AddSingleton<AzureDecodedClaimsPrincipalParser>()
            .AddSingleton<JwtPayloadClaimsPrincipalParser>();

    public static IServiceCollection AddHttpContextUserContext(this IServiceCollection services) =>
        services.AddScoped<IUserContext, UserContext>();
}