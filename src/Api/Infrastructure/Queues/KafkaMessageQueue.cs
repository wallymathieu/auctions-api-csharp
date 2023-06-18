using System.Text.Json;
using Wallymathieu.Auctions.Infrastructure.Queues;

namespace Wallymathieu.Auctions.Api.Infrastructure.Queues;
using Confluent.Kafka;
public class KafkaMessageQueue: IMessageQueue
{
    private readonly ProducerConfig _producerConfig;

    public KafkaMessageQueue(ProducerConfig producerConfig)
    {
        _producerConfig = producerConfig;
    }

    public bool Enabled => true;

    public async Task SendMessageAsync(string queueName, object command, CancellationToken cancellationToken)
    {
        using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
        await producer.ProduceAsync(queueName, new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = JsonSerializer.Serialize(command) }, cancellationToken);

        producer.Flush(TimeSpan.FromSeconds(1));
    }
}