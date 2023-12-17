using MediatR;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Glue class : Some would prefer to put these classes in an "Application" layer
/// </summary>
internal class CreateBidQueuePipeLineBehavior(IMessageQueue messageQueue, IUserContext userContext) :
    IPipelineBehavior<CreateBidCommand, Result<Bid,Errors>>
{

    public async Task<Result<Bid,Errors>> Handle(CreateBidCommand request, RequestHandlerDelegate<Result<Bid,Errors>> next, CancellationToken cancellationToken)
    {
        var result = await next();
        if (messageQueue.Enabled)
        {
            await messageQueue.SendMessageAsync(QueuesModule.BidResultQueueName,
                new UserIdDecorator<Result<Bid,Errors>?>(result, userContext.UserId), cancellationToken);
        }
        return result;
    }
}