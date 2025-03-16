using System.Text.Json;
using Azure.Storage.Queues;
using Wallymathieu.Auctions.Infrastructure.Json;
using Wallymathieu.Auctions.Infrastructure.Queues;

namespace Wallymathieu.Auctions.Infrastructure.Web.Queues;

internal sealed class AzureMessageQueue : IMessageQueue
{
    private readonly QueueServiceClient? _queueServiceClient;
    private readonly JsonSerializerOptions _serializerOptions;

    public AzureMessageQueue(IEnumerable<QueueServiceClient> clients)
    {
        _serializerOptions = new JsonSerializerOptions();
        _serializerOptions.AddAuctionConverters();
        _queueServiceClient = clients.FirstOrDefault();
    }

    public bool Enabled => _queueServiceClient != null;

    public async ValueTask SendMessageAsync(string queueName, object command, CancellationToken cancellationToken)
    {
        if (_queueServiceClient == null) throw new InvalidOperationException("Message queue is not enabled");
        var commandsQueue = _queueServiceClient.GetQueueClient(queueName);
        await commandsQueue.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        await commandsQueue.SendMessageAsync(JsonSerializer.Serialize(command, _serializerOptions), cancellationToken);
    }
}