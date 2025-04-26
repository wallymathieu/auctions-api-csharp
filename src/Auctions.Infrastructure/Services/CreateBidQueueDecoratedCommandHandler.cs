using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Decorator to add queue logic
/// </summary>
internal class CreateBidQueueDecoratedCommandHandler(
    ICreateBidCommandHandler commandHandler,
    IMessageQueue messageQueue,
    IUserContext userContext)
    : ICreateBidCommandHandler
{
    public async Task<Result<Bid, Errors>?> Handle(CreateBidCommand model,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.Handle(model, cancellationToken);
        if (messageQueue.Enabled)
        {
            await messageQueue.SendMessageAsync(QueuesModule.BidResultQueueName,
                new UserIdDecorator<Result<Bid, Errors>?>(result, userContext.UserId), cancellationToken);
        }

        return result;
    }
}