using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Cache.Data;
using Wallymathieu.Auctions.Infrastructure.Cache.Services;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Infrastructure.Services;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddAuctionQueryCached(this IServiceCollection services)
    {
        return services.AddAuctionQueryImplementation()
            .AddScoped<IAuctionQuery>(c=>new CachedAuctionQuery(
                c.GetRequiredService<IDistributedCache>(),
                c.GetRequiredService<AuctionDbContext>()));
    }
    public static IServiceCollection AddAuctionServicesCached(this IServiceCollection services) =>
        services.AddAuctionServicesImplementation()
            .AddScoped<ICreateAuctionCommandHandler>(c=>
                new CacheAwareCreateAuctionCommandHandler(
                    new CreateAuctionQueueDecoratedCommandHandler(
                    c.GetRequiredService<InnerService<ICreateAuctionCommandHandler>>().Service,
                        c.GetRequiredService<IMessageQueue>(),
                        c.GetRequiredService<IUserContext>()),
                    c.GetRequiredService<IDistributedCache>()))
            .AddScoped<ICreateBidCommandHandler>(c=>
                new CacheAwareCreateBidCommandHandler(
                    new CreateBidQueueDecoratedCommandHandler(
                        c.GetRequiredService<InnerService<ICreateBidCommandHandler>>().Service,
                        c.GetRequiredService<IMessageQueue>(),
                        c.GetRequiredService<IUserContext>()),
                    c.GetRequiredService<IDistributedCache>()));
}