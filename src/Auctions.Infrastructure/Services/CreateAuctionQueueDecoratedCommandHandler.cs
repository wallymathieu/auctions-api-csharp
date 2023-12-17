using MediatR;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Glue class: Some would prefer to put these classes in an "Application" layer
/// </summary>
internal class CreateAuctionQueueDecoratedCommandHandler:
    IPipelineBehavior<CreateAuctionCommand, Auction>
{
    private readonly IUserContext _userContext;
    private readonly IMessageQueue _messageQueue;

    public CreateAuctionQueueDecoratedCommandHandler(IMessageQueue messageQueue, IUserContext userContext)
    {
        _userContext = userContext;
        _messageQueue = messageQueue;
    }


    public async Task<Auction> Handle(CreateAuctionCommand request, RequestHandlerDelegate<Auction> next, CancellationToken cancellationToken)
    {
        var result = await next();
        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.AuctionResultQueueName,
                new UserIdDecorator<Auction>(result, _userContext.UserId), cancellationToken);
        }
        return result;
    }
}