using Wallymathieu.Auctions.Services;
using Wallymathieu.Auctions.Application.Data;

namespace Wallymathieu.Auctions.Application.Services;

/// <summary>
/// Glue class
/// </summary>
internal class CreateBidCommandHandler(
    IAuctionDbContext auctionDbContext,
    IUserContext userContext,
    ISystemClock systemClock)
    : ICreateBidCommandHandler
{
    public async Task<Result<Bid, Errors>?> Handle(CreateBidCommand model,
        CancellationToken cancellationToken = default)
    {
        var auction = await auctionDbContext.GetAuction(model.AuctionId, cancellationToken);
        if (auction is null) return Result.Error<Bid, Errors>(Errors.UnknownAuction);
        var result = auction.TryAddBid(model, userContext, systemClock);
        if (result.IsOk)
        {
            await auctionDbContext.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}