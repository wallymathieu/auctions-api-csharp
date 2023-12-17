using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Wallymathieu.Auctions.Infrastructure.Cache.Services;
internal class CreateBidCachePipeLineBehavior(IDistributedCache cache):
    IPipelineBehavior<CreateBidCommand, Result<Bid,Errors>>
{
    public async Task<Result<Bid, Errors>> Handle(CreateBidCommand request, RequestHandlerDelegate<Result<Bid, Errors>> next, CancellationToken cancellationToken)
    {
        var res = await next();
        await cache.RemoveAsync(CacheKeys.Auctions, cancellationToken);
        return res;
    }
}