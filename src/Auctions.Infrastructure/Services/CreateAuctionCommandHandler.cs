using Marten;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Glue class: Some would prefer to put these classes in an "Application" layer
/// </summary>
internal class CreateAuctionCommandHandler(
    IDocumentSession documentSession,
    IUserContext userContext,
    IMessageQueue messageQueue)
    : ICreateAuctionCommandHandler
{
    public async Task<Auction> Handle(CreateAuctionCommand model, CancellationToken cancellationToken = default)
    {
        var auction = Auction.Create(model, userContext);
        documentSession.Insert(auction);
        await documentSession.SaveChangesAsync(cancellationToken);
        if (messageQueue.Enabled)
        {
            await messageQueue.SendMessageAsync(QueuesModule.AuctionResultQueueName,
                new UserIdDecorator<Auction>(auction, userContext.UserId), cancellationToken);
        }
        return auction;
    }
}