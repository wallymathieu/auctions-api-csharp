using Auctions.Domain;
using Auctions.Infrastructure.Cache;
using Auctions.Models;
using Auctions.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Auctions.Infrastructure.Services.Cache;

internal class CacheAwareCreateAuctionCommandHandler:ICreateAuctionCommandHandler
{
    private readonly CreateAuctionCommandHandler _createAuctionCommandHandler;
    private readonly IDistributedCache _cache;
    public CacheAwareCreateAuctionCommandHandler(CreateAuctionCommandHandler createAuctionCommandHandler, IDistributedCache cache)
    {
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _cache = cache;
    }

    public async Task<AuctionModel> Handle(UserId userId, CreateAuctionModel model)
    {
        var res = await _createAuctionCommandHandler.Handle(userId, model);
        await _cache.RemoveAsync(CacheKeys.Auctions); 
        return res;
    }
}