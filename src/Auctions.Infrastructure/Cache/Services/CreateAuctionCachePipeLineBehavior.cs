using Mediator;
using Microsoft.Extensions.Caching.Distributed;

namespace Wallymathieu.Auctions.Infrastructure.Cache.Services;
internal class CreateAuctionCachePipeLineBehavior(IDistributedCache cache):
    IPipelineBehavior<CreateAuctionCommand, Auction>
{
    public async ValueTask<Auction> Handle(CreateAuctionCommand message, CancellationToken cancellationToken, MessageHandlerDelegate<CreateAuctionCommand, Auction> next)
    {
        var res = await next(message, cancellationToken);
        await cache.RemoveAsync(CacheKeys.Auctions, cancellationToken);
        return res;
    }
}