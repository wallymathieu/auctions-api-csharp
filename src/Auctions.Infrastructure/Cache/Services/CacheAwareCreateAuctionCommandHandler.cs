using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Wallymathieu.Auctions.Infrastructure.Cache.Services;
internal class CacheAwareCreateAuctionCommandHandler:
    IPipelineBehavior<CreateAuctionCommand, Auction>
{
    private readonly IDistributedCache _cache;
    public CacheAwareCreateAuctionCommandHandler(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<Auction> Handle(CreateAuctionCommand request, RequestHandlerDelegate<Auction> next, CancellationToken cancellationToken)
    {
        var res = await next();
        await _cache.RemoveAsync(CacheKeys.Auctions, cancellationToken);
        return res;
    }
}