using Microsoft.Extensions.DependencyInjection;

namespace Wallymathieu.Auctions.Infrastructure.Models;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuctionMapper(this IServiceCollection services) =>
        services.AddSingleton<Auctions.Models.V1.AuctionMapper>()
                .AddSingleton<Auctions.Models.V2.AuctionMapper>();
}