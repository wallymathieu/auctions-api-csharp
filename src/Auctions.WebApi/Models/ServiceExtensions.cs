using Wallymathieu.Auctions.Api.Models.V2;

namespace Wallymathieu.Auctions.Api.Models;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuctionMapper(this IServiceCollection services) =>
        services.AddSingleton<V1.AuctionMapper>()
                .AddSingleton<AuctionMapper>();
}