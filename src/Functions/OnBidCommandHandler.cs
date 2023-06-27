namespace Wallymathieu.Auctions.Functions;
public class OnBidCommandHandler
{
    private readonly ICreateBidCommandHandler _createBidCommandHandler;
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ScopedUserContext _userContext;

    public OnBidCommandHandler(ILoggerFactory loggerFactory,
        ICreateBidCommandHandler createBidCommandHandler,
        JsonSerializerOptions serializerOptions,
        ScopedUserContext userContext)
    {
        _createBidCommandHandler = createBidCommandHandler;
        _serializerOptions = serializerOptions;
        _logger = loggerFactory.CreateLogger<OnBidCommandHandler>();
        _userContext = userContext;
    }

    [Function("OnBidCommand")]
    public async Task Run(
        [QueueTrigger(QueuesModule.BidCommandQueueName, Connection = "AzureWebJobsStorage")]
        string commandString, CancellationToken cancellationToken)
    {
        var command = JsonSerializer.Deserialize<UserIdDecorator<CreateBidCommand>>(commandString, _serializerOptions);
        if (command == null) throw new NullReferenceException(nameof(command));
        _userContext.UserId = command.UserId;
        _logger.LogInformation($"bid received");
        var result = await _createBidCommandHandler.Handle(command.Command, cancellationToken);
        if (result is null) return;
        result.Match(ok => _logger.LogInformation("bid processed successfully {Result}", ok),
            error => _logger.LogInformation("bid processed {Error}", error));
    }
}