using MediatR;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Glue class : Some would prefer to put these classes in an "Application" layer
/// </summary>
internal class CreateBidQueueDecoratedCommandHandler :
    IPipelineBehavior<CreateBidCommand, Result<Bid,Errors>>
{
    private readonly IMessageQueue _messageQueue;
    private readonly IUserContext _userContext;
    public CreateBidQueueDecoratedCommandHandler(IMessageQueue messageQueue, IUserContext userContext)
    {
        _messageQueue = messageQueue;
        _userContext = userContext;
    }

    public async Task<Result<Bid,Errors>> Handle(CreateBidCommand request, RequestHandlerDelegate<Result<Bid,Errors>> next, CancellationToken cancellationToken)
    {
        var result = await next();
        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.BidResultQueueName,
                new UserIdDecorator<Result<Bid,Errors>?>(result, _userContext.UserId), cancellationToken);
        }
        return result;
    }
}