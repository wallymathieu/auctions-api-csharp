using Microsoft.Extensions.Caching.Distributed;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Infrastructure.Cache;
namespace Wallymathieu.Auctions.Infrastructure.Services.Cache;
using ICreateAuctionCommandHandler= ICommandHandler<CreateAuctionCommand, TimedAscendingAuction>;
internal class CacheAwareCreateAuctionCommandHandler: ICommandHandler<CreateAuctionCommand, TimedAscendingAuction>
{
    private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
    private readonly IDistributedCache _cache;
    public CacheAwareCreateAuctionCommandHandler(ICreateAuctionCommandHandler createAuctionCommandHandler, IDistributedCache cache)
    {
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _cache = cache;
    }

    public async Task<TimedAscendingAuction> Handle(CreateAuctionCommand model, CancellationToken cancellationToken)
    {
        var res = await _createAuctionCommandHandler.Handle(model, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Auctions, cancellationToken);
        return res;
    }
}