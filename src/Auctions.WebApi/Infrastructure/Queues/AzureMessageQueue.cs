using System.Text.Json;
using Azure.Storage.Queues;
using Wallymathieu.Auctions.Infrastructure.Json;
using Wallymathieu.Auctions.Infrastructure.Queues;

namespace Wallymathieu.Auctions.Api.Infrastructure.Queues;

public class AzureMessageQueue:IMessageQueue
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly QueueServiceClient _queueServiceClient;

    public AzureMessageQueue(QueueServiceClient client)
    {
        _serializerOptions = new JsonSerializerOptions();
        _serializerOptions.AddAuctionConverters();
        _queueServiceClient = client;
    }

    public async Task SendMessageAsync(string queueName, object command, CancellationToken cancellationToken)
    {
        if (_queueServiceClient == null) throw new InvalidOperationException("Message queue is not enabled");
        var commandsQueue = _queueServiceClient.GetQueueClient(queueName);
        await commandsQueue.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        await commandsQueue.SendMessageAsync(JsonSerializer.Serialize(command, _serializerOptions), cancellationToken);
    }
}