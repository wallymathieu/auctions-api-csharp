using Microsoft.Extensions.DependencyInjection;

namespace Wallymathieu.Auctions.Application.Models;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuctionMapper(this IServiceCollection services) => services.AddSingleton<AuctionMapper>();
}