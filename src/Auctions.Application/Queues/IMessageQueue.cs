namespace Wallymathieu.Auctions.Application.Queues;

public interface IMessageQueue
{
    bool Enabled { get; }
    Task SendMessageAsync(string queueName, object command, CancellationToken cancellationToken);
}