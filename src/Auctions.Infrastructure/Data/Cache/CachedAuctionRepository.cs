using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Wallymathieu.Auctions.Data;
using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Infrastructure.Cache;

namespace Wallymathieu.Auctions.Infrastructure.Data.Cache;

/// <summary>
/// https://learn.microsoft.com/en-us/azure/architecture/patterns/cache-aside
/// </summary>
public class CachedAuctionRepository:IAuctionRepository
{
    private readonly IDistributedCache _cache;
    private readonly AuctionRepository _auctionRepository;
    public CachedAuctionRepository(IDistributedCache cache, AuctionRepository auctionRepository)
    {
        _cache = cache;
        _auctionRepository = auctionRepository;
    }

    public Task<TimedAscendingAuction?> GetAuctionAsync(long auctionId) => _auctionRepository.GetAuctionAsync(auctionId);

    public async Task<IReadOnlyCollection<TimedAscendingAuction>> GetAuctionsAsync()
    {
        var auctionsJson = await _cache.GetStringAsync(CacheKeys.Auctions);
        if (auctionsJson != null)
        {
            // We have cached data, deserialize the JSON data.
            return JsonSerializer.Deserialize<List<TimedAscendingAuction>>(auctionsJson);
        }
        else
        {
            // There's nothing in the cache, retrieve data from the repository and cache it for one hour.
            var auctions = await _auctionRepository.GetAuctionsAsync();
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