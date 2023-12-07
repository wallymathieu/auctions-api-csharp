using Wallymathieu.Auctions.Application.Data;
using Wallymathieu.Auctions.Application.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Application.Services;

/// <summary>
/// Glue class: Some would prefer to put these classes in a "Application" layer
/// </summary>
internal class CreateAuctionCommandHandler:ICreateAuctionCommandHandler
{
    private readonly IAuctionDbContext _auctionDbContext;
    private readonly IUserContext _userContext;
    private readonly IMessageQueue _messageQueue;

    public CreateAuctionCommandHandler(IAuctionDbContext auctionDbContext, IUserContext userContext, IMessageQueue messageQueue)
    {
        _auctionDbContext = auctionDbContext;
        _userContext = userContext;
        _messageQueue = messageQueue;
    }

    public async Task<Auction> Handle(CreateAuctionCommand model, CancellationToken cancellationToken = default)
    {
        var auction = Auction.Create(model, _userContext);
        await _auctionDbContext.AddAsync(auction, cancellationToken);
        await _auctionDbContext.SaveChangesAsync(cancellationToken);
        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.AuctionResultQueueName,
                new UserIdDecorator<Auction>(auction, _userContext.UserId), cancellationToken);
        }
        return auction;
    }
}