using Microsoft.Extensions.Caching.Distributed;
using Wallymathieu.Auctions.Infrastructure.Cache;
using Wallymathieu.Auctions.Infrastructure.CommandHandlers;

namespace Wallymathieu.Auctions.Infrastructure.Services.Cache;
internal class CacheAwareCreateAuctionCommandHandler: ICommandHandler<CreateAuctionCommand, Auction>
{
    private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
    private readonly IDistributedCache _cache;
    public CacheAwareCreateAuctionCommandHandler(ICreateAuctionCommandHandler createAuctionCommandHandler, IDistributedCache cache)
    {
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _cache = cache;
    }

    public async Task<Auction> Handle(CreateAuctionCommand model, CancellationToken cancellationToken)
    {
        var res = await _createAuctionCommandHandler.Handle(model, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Auctions, cancellationToken);
        return res;
    }
}