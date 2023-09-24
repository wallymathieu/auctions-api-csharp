namespace Wallymathieu.Auctions.Infrastructure.Queues;

public interface IMessageQueue
{
    Task SendMessageAsync(string queueName, object command, CancellationToken cancellationToken);
}