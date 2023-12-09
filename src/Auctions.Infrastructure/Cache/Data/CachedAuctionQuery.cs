using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Infrastructure.Cache.Data;

/// <summary>
/// https://learn.microsoft.com/en-us/azure/architecture/patterns/cache-aside
/// </summary>
public class CachedAuctionQuery:AuctionQuery
{
    private readonly IDistributedCache _cache;
    public CachedAuctionQuery(IDistributedCache cache, AuctionDbContext dbContext): base(dbContext)
    {
        _cache = cache;
    }

    public override async Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken)
    {
        var auctionsJson = await _cache.GetStringAsync(CacheKeys.Auctions);
        if (auctionsJson != null)
        {
            // We have cached data, deserialize the JSON data.
            return JsonSerializer.Deserialize<List<Auction>>(auctionsJson);
        }
        else
        {
            // There's nothing in the cache, retrieve data from the repository and cache it for one hour.
            var auctions = await base.GetAuctionsAsync(cancellationToken);
            auctionsJson = JsonSerializer.Serialize(auctions);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };
            await this._cache.SetStringAsync(CacheKeys.Auctions, auctionsJson, cacheOptions);
            return auctions;
        }
    }
}