using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Wallymathieu.Auctions.Infrastructure.Cache.Services;
internal class CreateAuctionCachePipeLineBehavior(IDistributedCache cache):
    IPipelineBehavior<CreateAuctionCommand, Auction>
{
    public async Task<Auction> Handle(CreateAuctionCommand request, RequestHandlerDelegate<Auction> next, CancellationToken cancellationToken)
    {
        var res = await next();
        await cache.RemoveAsync(CacheKeys.Auctions, cancellationToken);
        return res;
    }
}