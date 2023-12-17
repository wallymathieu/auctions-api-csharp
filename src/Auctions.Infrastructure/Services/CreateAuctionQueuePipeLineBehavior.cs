using MediatR;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Glue class: Some would prefer to put these classes in an "Application" layer
/// </summary>
internal class CreateAuctionQueuePipeLineBehavior(IMessageQueue messageQueue, IUserContext userContext):
    IPipelineBehavior<CreateAuctionCommand, Auction>
{

    public async Task<Auction> Handle(CreateAuctionCommand request, RequestHandlerDelegate<Auction> next, CancellationToken cancellationToken)
    {
        var result = await next();
        if (messageQueue.Enabled)
        {
            await messageQueue.SendMessageAsync(QueuesModule.AuctionResultQueueName,
                new UserIdDecorator<Auction>(result, userContext.UserId), cancellationToken);
        }
        return result;
    }
}