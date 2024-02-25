using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;
using Marten;
namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Glue class : Some would prefer to put these classes in an "Application" layer
/// </summary>
internal class CreateBidCommandHandler(
    IDocumentSession documentSession,
    IUserContext userContext,
    ISystemClock systemClock,
    IMessageQueue messageQueue)
    : ICreateBidCommandHandler
{
    public async Task<Result<Bid, Errors>?> Handle(CreateBidCommand model, CancellationToken cancellationToken = default)
    {
        var auction = await documentSession.GetAuction(model.AuctionId, cancellationToken);
        if (auction is null) return Result<Bid, Errors>.Error(Errors.UnknownAuction);
        var result = auction.TryAddBid(model, userContext, systemClock);
        if (result.IsOk)
        {
            await documentSession.SaveChangesAsync(cancellationToken);
        }
        if (messageQueue.Enabled)
        {
            await messageQueue.SendMessageAsync(QueuesModule.BidResultQueueName,
                new UserIdDecorator<Result<Bid,Errors>?>(result, userContext.UserId), cancellationToken);
        }
        return result;
    }
}