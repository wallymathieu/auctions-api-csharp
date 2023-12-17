using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Glue class : Some would prefer to put these classes in a "Application" layer
/// </summary>
internal class CreateBidQueueDecoratedCommandHandler : ICreateBidCommandHandler
{
    private readonly ICreateBidCommandHandler _commandHandler;
    private readonly IMessageQueue _messageQueue;
    private readonly IUserContext _userContext;
    public CreateBidQueueDecoratedCommandHandler(ICreateBidCommandHandler commandHandler, IMessageQueue messageQueue, IUserContext userContext)
    {
        _commandHandler = commandHandler;
        _messageQueue = messageQueue;
        _userContext = userContext;
    }

    public async Task<Result<Bid, Errors>?> Handle(CreateBidCommand model, CancellationToken cancellationToken = default)
    {
        var result = await _commandHandler.Handle(model, cancellationToken);
        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.BidResultQueueName,
                new UserIdDecorator<Result<Bid,Errors>?>(result, _userContext.UserId), cancellationToken);
        }
        return result;
    }
}