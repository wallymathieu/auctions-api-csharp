using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Glue class : Some would prefer to put these classes in a "Application" layer
/// </summary>
internal class CreateBidCommandHandler : ICreateBidCommandHandler
{
    private readonly AuctionDbContext _auctionDbContext;
    private readonly IUserContext _userContext;
    private readonly ISystemClock _systemClock;
    private readonly IMessageQueue _messageQueue;

    public CreateBidCommandHandler(AuctionDbContext auctionDbContext, IUserContext userContext, ISystemClock systemClock, IMessageQueue messageQueue)
    {
        _auctionDbContext = auctionDbContext;
        _userContext = userContext;
        _systemClock = systemClock;
        _messageQueue = messageQueue;
    }

    public async Task<Result<Bid, Errors>?> Handle(CreateBidCommand model, CancellationToken cancellationToken = default)
    {
        var auction = await _auctionDbContext.GetAuction(model.AuctionId, cancellationToken);
        if (auction is null) return Result<Bid, Errors>.Error(Errors.UnknownAuction);
        var result = auction.TryAddBid(model, _userContext, _systemClock);
        if (result.IsOk)
        {
            await _auctionDbContext.SaveChangesAsync(cancellationToken);
        }
        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.BidResultQueueName,
                new UserIdDecorator<Result<Bid,Errors>?>(result, _userContext.UserId), cancellationToken);
        }
        return result;
    }
}