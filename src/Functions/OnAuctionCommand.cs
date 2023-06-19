namespace Wallymathieu.Auctions.Functions;
public class OnAuctionCommandHandler
{
    private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ScopedUserContext _userContext;

    public OnAuctionCommandHandler(ILoggerFactory loggerFactory,
        ICreateAuctionCommandHandler createAuctionCommandHandler,
        JsonSerializerOptions serializerOptions,
        ScopedUserContext userContext)
    {
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _serializerOptions = serializerOptions;
        _logger = loggerFactory.CreateLogger<OnAuctionCommandHandler>();
        _userContext = userContext;
    }

    [Function("OnAuctionCommand")]
    public async Task Run(
        [QueueTrigger(QueuesModule.AuctionCommandQueueName, Connection = "AzureWebJobsStorage")]
        string commandString, CancellationToken cancellationToken)
    {
        var command = JsonSerializer.Deserialize<UserIdDecorator<CreateAuctionCommand>>(commandString, _serializerOptions);
        _userContext.UserId = command.UserId;
        _logger.LogInformation("Create auction command received");
        if (command == null) throw new NullReferenceException(nameof(command));
        var result = await _createAuctionCommandHandler.Handle(command.Command, cancellationToken);
        _logger.LogInformation("Create auction command processed {AuctionId}", result.Id);
    }

    [Function("OnAuctionCommandKafka")]
    public async Task RunKafka(
        [KafkaTrigger("broker",
            QueuesModule.AuctionCommandQueueName,
            Username = "KAFKA_USERNAME",
            Password = "KAFKA_PASSWORD",
            Protocol = BrokerProtocol.Plaintext,
            AuthenticationMode = BrokerAuthenticationMode.Plain,
            ConsumerGroup = "$Default")] string commandString,
        CancellationToken cancellationToken)
    {
        var command = JsonSerializer.Deserialize<CreateAuctionCommand>(commandString, _serializerOptions);
        _logger.LogInformation("Create auction command received");
        if (command == null) throw new NullReferenceException(nameof(command));
        var result = await _createAuctionCommandHandler.Handle(command, cancellationToken);
        _logger.LogInformation("Create auction command processed {AuctionId}", result.Id);
    }
}