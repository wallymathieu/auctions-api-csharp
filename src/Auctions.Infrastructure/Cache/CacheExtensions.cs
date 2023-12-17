using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Cache.Data;
using Wallymathieu.Auctions.Infrastructure.Cache.Services;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Services;

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
    public static IServiceCollection AddAuctionServicesCached(this IServiceCollection services)
    {
        services.AddAuctionServicesImplementation();
        services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssemblyContaining<CreateAuctionCachePipeLineBehavior>();
            c.AddBehavior<CreateAuctionQueuePipeLineBehavior>();
            c.AddBehavior<CreateBidQueuePipeLineBehavior>();
            c.AddBehavior<CreateAuctionCachePipeLineBehavior>();
            c.AddBehavior<CreateBidCachePipeLineBehavior>();
        });
        return services;
    }
}