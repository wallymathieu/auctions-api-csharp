using Mediator;
using Microsoft.Extensions.Caching.Distributed;

namespace Wallymathieu.Auctions.Infrastructure.Cache.Services;
internal class CreateBidCachePipeLineBehavior(IDistributedCache cache):
    IPipelineBehavior<CreateBidCommand, Result<Bid,Errors>>
{
    public async ValueTask<Result<Bid, Errors>> Handle(CreateBidCommand message, CancellationToken cancellationToken, MessageHandlerDelegate<CreateBidCommand, Result<Bid, Errors>> next)
    {
        var res = await next(message, cancellationToken);
        await cache.RemoveAsync(CacheKeys.Auctions, cancellationToken);
        return res;
    }
}