namespace Wallymathieu.Auctions.Functions;
public class OnBidCommandHandler
{
    private readonly ICreateBidCommandHandler _createBidCommandHandler;
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public OnBidCommandHandler(ILoggerFactory loggerFactory, ICreateBidCommandHandler createBidCommandHandler, JsonSerializerOptions serializerOptions)
    {
        _createBidCommandHandler = createBidCommandHandler;
        _serializerOptions = serializerOptions;
        _logger = loggerFactory.CreateLogger<OnBidCommandHandler>();
    }

    [Function("OnBidCommand")]
    public async Task Run(
        [QueueTrigger(QueuesModule.BidCommandQueueName, Connection = "AzureWebJobsStorage")]
        string commandString, CancellationToken cancellationToken)
    {
        var command = JsonSerializer.Deserialize<CreateBidCommand>(commandString, _serializerOptions);
        _logger.LogInformation($"bid received");
        if (command == null) throw new NullReferenceException(nameof(command));
        var result = await _createBidCommandHandler.Handle(command, cancellationToken);
        switch (result)
        {
            case Ok<Bid,Errors> ok:
                _logger.LogInformation("bid processed successfully {Result}", ok);
                break;
            case Error<Bid,Errors> error:
                _logger.LogInformation("bid processed {Error}", error);
                break;
        }
    }
    [Function("OnBidCommandKafka")]
    public async Task RunKafka(
        [KafkaTrigger("broker",
            QueuesModule.BidCommandQueueName,
            Username = "KAFKA_USERNAME",
            Password = "KAFKA_PASSWORD",
            Protocol = BrokerProtocol.SaslSsl,
            AuthenticationMode = BrokerAuthenticationMode.Plain,
            ConsumerGroup = "$Default")] string commandString, CancellationToken cancellationToken)
    {
        var command = JsonSerializer.Deserialize<CreateBidCommand>(commandString, _serializerOptions);
        _logger.LogInformation($"bid received");
        if (command == null) throw new NullReferenceException(nameof(command));
        var result = await _createBidCommandHandler.Handle(command, cancellationToken);
        switch (result)
        {
            case Ok<Bid,Errors> ok:
                _logger.LogInformation("bid processed successfully {Result}", ok);
                break;
            case Error<Bid,Errors> error:
                _logger.LogInformation("bid processed {Error}", error);
                break;
        }
    }
}