using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Wallymathieu.Auctions.Infrastructure.Cache.Services;
internal class CreateBidCachePipeLineBehavior:
    IPipelineBehavior<CreateBidCommand, Result<Bid,Errors>>
{
    private readonly IDistributedCache _cache;

    public CreateBidCachePipeLineBehavior(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<Result<Bid, Errors>> Handle(CreateBidCommand request, RequestHandlerDelegate<Result<Bid, Errors>> next, CancellationToken cancellationToken)
    {
        var res = await next();
        await _cache.RemoveAsync(CacheKeys.Auctions, cancellationToken);
        return res;
    }
}