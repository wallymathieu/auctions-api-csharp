using Microsoft.Extensions.DependencyInjection;

namespace Wallymathieu.Auctions.Infrastructure.Models;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuctionMapper(this IServiceCollection services) => services.AddSingleton<AuctionMapper>();
}