using Microsoft.Extensions.Caching.Distributed;
using Wallymathieu.Auctions.Infrastructure.Cache;

namespace Wallymathieu.Auctions.Infrastructure.Services.Cache;
internal class CacheAwareCreateBidCommandHandler: ICreateBidCommandHandler
{
    private readonly ICreateBidCommandHandler _createBidCommandHandler;
    private readonly IDistributedCache _cache;

    public CacheAwareCreateBidCommandHandler(ICreateBidCommandHandler createBidCommandHandler, IDistributedCache cache)
    {
        _createBidCommandHandler = createBidCommandHandler;
        _cache = cache;
    }

    public async Task<IResult<Bid,Errors>> Handle(CreateBidCommand model, CancellationToken token)
    {
        var res = await _createBidCommandHandler.Handle(model, token);
        await _cache.RemoveAsync(CacheKeys.Auctions, token);
        return res;
    }
}