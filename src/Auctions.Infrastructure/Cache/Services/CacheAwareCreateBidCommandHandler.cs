using Microsoft.Extensions.Caching.Distributed;
using Wallymathieu.Auctions.Application.Services;
using Wallymathieu.Auctions.Infrastructure.Services;

namespace Wallymathieu.Auctions.Infrastructure.Cache.Services;

internal sealed class CacheAwareCreateBidCommandHandler(
    ICreateBidCommandHandler createBidCommandHandler,
    IDistributedCache cache)
    : ICreateBidCommandHandler
{
    public async Task<Result<Bid, Errors>?> Handle(CreateBidCommand model,
        CancellationToken cancellationToken = default)
    {
        var res = await createBidCommandHandler.Handle(model, cancellationToken);
        await cache.RemoveAsync(CacheKeys.Auctions, cancellationToken);
        return res;
    }
}