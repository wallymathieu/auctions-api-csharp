using Wallymathieu.Auctions.Application.Data;
using Wallymathieu.Auctions.Application.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Application.Services;

/// <summary>
/// Glue class
/// </summary>
internal class CreateBidCommandHandler(
    IAuctionDbContext auctionDbContext,
    IUserContext userContext,
    ISystemClock systemClock,
    IMessageQueue messageQueue)
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

        if (messageQueue.Enabled)
        {
            await messageQueue.SendMessageAsync(QueuesModule.BidResultQueueName,
                new UserIdDecorator<Result<Bid, Errors>?>(result, userContext.UserId), cancellationToken);
        }

        return result;
    }
}