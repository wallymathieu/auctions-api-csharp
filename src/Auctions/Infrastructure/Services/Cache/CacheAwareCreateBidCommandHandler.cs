using Microsoft.Extensions.Caching.Distributed;
using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Infrastructure.Cache;
using Wallymathieu.Auctions.Models;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services.Cache;

internal class CacheAwareCreateBidCommandHandler:ICreateBidCommandHandler
{
    private readonly CreateBidCommandHandler _createBidCommandHandler;
    private readonly IDistributedCache _cache;

    public CacheAwareCreateBidCommandHandler(CreateBidCommandHandler createBidCommandHandler, IDistributedCache cache)
    {
        _createBidCommandHandler = createBidCommandHandler;
        _cache = cache;
    }

    public async Task<(CreateBidCommandResult Result, Errors Errors)> Handle(long auctionId, UserId userId, CreateBidModel model)
    {
        var res = await _createBidCommandHandler.Handle(auctionId, userId, model);
        await _cache.RemoveAsync(CacheKeys.Auctions); 
        return res;
    }
}