using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Decorator to add queue logic
/// </summary>
internal class CreateAuctionQueueDecoratedCommandHandler(
    ICreateAuctionCommandHandler commandHandler,
    IMessageQueue messageQueue,
    IUserContext userContext)
    : ICreateAuctionCommandHandler
{
    public async Task<Auction?> Handle(CreateAuctionCommand model, CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.Handle(model, cancellationToken);
        if (messageQueue.Enabled && result is not null)
        {
            await messageQueue.SendMessageAsync(QueuesModule.AuctionResultQueueName,
                new UserIdDecorator<Auction>(result, userContext.UserId), cancellationToken);
        }
        return result;
    }
}