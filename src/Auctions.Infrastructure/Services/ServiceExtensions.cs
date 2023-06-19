using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wallymathieu.Auctions.Infrastructure.Services.Cache;
using Wallymathieu.Auctions.Services;
namespace Wallymathieu.Auctions.Infrastructure.Services;
public static class ServiceExtensions
{
    private static IServiceCollection AddAuctionServicesImplementation(this IServiceCollection services)
    {
        services.TryAddSingleton<ITime, Time>();
        services.TryAddScoped<CreateAuctionCommandHandler>();
        services.TryAddScoped<CreateBidCommandHandler>();
        services.TryAddScoped<IInnerService<ICreateAuctionCommandHandler>>(c=>
            new InnerService<ICreateAuctionCommandHandler>(c.GetRequiredService<CreateAuctionCommandHandler>()));
        services.TryAddScoped<IInnerService<ICreateBidCommandHandler>>(c=>
            new InnerService<ICreateBidCommandHandler>(c.GetRequiredService<CreateBidCommandHandler>()));
        return services;
    }

    public static IServiceCollection AddAuctionServicesNoCache(this IServiceCollection services) =>
        AddAuctionServicesImplementation(services)
            .AddScoped<ICreateAuctionCommandHandler>(c=>
                    c.GetRequiredService<IInnerService<ICreateAuctionCommandHandler>>().Service)
            .AddScoped<ICreateBidCommandHandler>(c=>
                    c.GetRequiredService<IInnerService<ICreateBidCommandHandler>>().Service);

    public static IServiceCollection AddAuctionServicesCached(this IServiceCollection services) =>
        AddAuctionServicesImplementation(services)
            .AddScoped<ICreateAuctionCommandHandler>(c=>
                new CacheAwareCreateAuctionCommandHandler(
                    c.GetRequiredService<IInnerService<ICreateAuctionCommandHandler>>().Service,
                    c.GetRequiredService<IDistributedCache>()))
            .AddScoped<ICreateBidCommandHandler>(c=>
                new CacheAwareCreateBidCommandHandler(
                    c.GetRequiredService<IInnerService<ICreateBidCommandHandler>>().Service,
                    c.GetRequiredService<IDistributedCache>()));
}