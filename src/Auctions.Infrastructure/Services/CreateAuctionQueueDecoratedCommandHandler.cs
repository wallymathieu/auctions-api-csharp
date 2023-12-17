using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

/// <summary>
/// Glue class: Some would prefer to put these classes in a "Application" layer
/// </summary>
internal class CreateAuctionQueueDecoratedCommandHandler:ICreateAuctionCommandHandler
{
    private readonly ICreateAuctionCommandHandler _commandHandler;
    private readonly IUserContext _userContext;
    private readonly IMessageQueue _messageQueue;

    public CreateAuctionQueueDecoratedCommandHandler(ICreateAuctionCommandHandler commandHandler, IMessageQueue messageQueue, IUserContext userContext)
    {
        _commandHandler = commandHandler;
        _userContext = userContext;
        _messageQueue = messageQueue;
    }

    public async Task<Auction?> Handle(CreateAuctionCommand model, CancellationToken cancellationToken)
    {
        var result = await _commandHandler.Handle(model, cancellationToken);
        if (_messageQueue.Enabled && result is not null)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.AuctionResultQueueName,
                new UserIdDecorator<Auction>(result, _userContext.UserId), cancellationToken);
        }
        return result;
    }
}