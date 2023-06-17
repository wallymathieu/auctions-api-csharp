namespace Wallymathieu.Auctions.Infrastructure.Queues;

public interface IMessageQueue
{
    bool Enabled { get; }
    Task SendMessageAsync(string queueName, object command, CancellationToken token);
}