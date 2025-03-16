namespace Wallymathieu.Auctions.Infrastructure.Queues;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public interface IMessageQueue
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    bool Enabled { get; }
    ValueTask SendMessageAsync(string queueName, object command, CancellationToken cancellationToken);
}