using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Infrastructure.Cache.Data;

/// <summary>
/// https://learn.microsoft.com/en-us/azure/architecture/patterns/cache-aside
/// </summary>
public class CachedAuctionQuery(IDistributedCache cache, CacheConfiguration options, AuctionDbContext dbContext): AuctionQuery(dbContext)
{
    public override async Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken=default)
    {
        var auctionsJson = await cache.GetStringAsync(CacheKeys.Auctions, token:cancellationToken);
        if (auctionsJson != null)
        {
            // We have cached data, deserialize the JSON data.
            return JsonSerializer.Deserialize<List<Auction>>(auctionsJson)!;
        }
        else
        {
            // There's nothing in the cache, retrieve data from the repository and cache it for one hour.
            var auctions = await base.GetAuctionsAsync(cancellationToken);
            auctionsJson = JsonSerializer.Serialize(auctions);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = options.DefaultExpiration,
                SlidingExpiration = options.SlidingExpiration
            };
            await cache.SetStringAsync(key:CacheKeys.Auctions,
                value:auctionsJson,
                options:cacheOptions, token:cancellationToken);
            return auctions;
        }
    }
}