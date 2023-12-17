using Microsoft.Extensions.Caching.Distributed;
using Wallymathieu.Auctions.Infrastructure.Services;

namespace Wallymathieu.Auctions.Infrastructure.Cache.Services;
internal class CacheAwareCreateAuctionCommandHandler(
    ICreateAuctionCommandHandler createAuctionCommandHandler,
    IDistributedCache cache)
    : ICreateAuctionCommandHandler
{
    public async Task<Auction> Handle(CreateAuctionCommand model, CancellationToken cancellationToken=default)
    {
        var res = await createAuctionCommandHandler.Handle(model, cancellationToken);
        await cache.RemoveAsync(CacheKeys.Auctions, cancellationToken);
        return res;
    }
}