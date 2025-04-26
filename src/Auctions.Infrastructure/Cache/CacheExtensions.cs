using Mediator;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            .AddScoped<IAuctionQuery>(c => new CachedAuctionQuery(
                c.GetRequiredService<IDistributedCache>(),
                c.GetRequiredService<IOptions<CacheConfiguration>>().Value ?? new CacheConfiguration(),
                c.GetRequiredService<AuctionDbContext>()));
    }
    public static IServiceCollection AddAuctionServicesCached(this IServiceCollection services)
    {
        services.AddAuctionServicesImplementation();
        services.AddMediator(c =>
        {
            c.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services
            .AddScoped<IPipelineBehavior<CreateAuctionCommand, Auction>, CreateAuctionQueuePipeLineBehavior>()
            .AddScoped<IPipelineBehavior<CreateBidCommand, Result<Bid,Errors>>, CreateBidQueuePipeLineBehavior>()
            .AddScoped<IPipelineBehavior<CreateAuctionCommand, Auction>, CreateAuctionCachePipeLineBehavior>()
            .AddScoped<IPipelineBehavior<CreateBidCommand, Result<Bid,Errors>>, CreateBidCachePipeLineBehavior>();
        return services;
    }
}