using Wallymathieu.Auctions.Application.Data;
using Wallymathieu.Auctions.Application.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Application.Services;

/// <summary>
/// Glue class
/// </summary>
internal class CreateAuctionCommandHandler(
    IAuctionDbContext auctionDbContext,
    IUserContext userContext,
    IMessageQueue messageQueue)
    : ICreateAuctionCommandHandler
{
    public async Task<Auction> Handle(CreateAuctionCommand model, CancellationToken cancellationToken = default)
    {
        var auction = Auction.Create(model, userContext);
        await auctionDbContext.AddAsync(auction, cancellationToken);
        await auctionDbContext.SaveChangesAsync(cancellationToken);
        if (messageQueue.Enabled)
        {
            await messageQueue.SendMessageAsync(QueuesModule.AuctionResultQueueName,
                new UserIdDecorator<Auction>(auction, userContext.UserId), cancellationToken);
        }
        return auction;
    }
}