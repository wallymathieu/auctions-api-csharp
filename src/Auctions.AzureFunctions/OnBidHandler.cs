namespace Wallymathieu.Auctions.Functions;
public class OnBidHandler
{
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ScopedUserContext _userContext;

    public OnBidHandler(ILoggerFactory loggerFactory,
        JsonSerializerOptions serializerOptions,
        ScopedUserContext userContext)
    {
        _serializerOptions = serializerOptions;
        _logger = loggerFactory.CreateLogger<OnBidHandler>();
        _userContext = userContext;
    }

    [Function("OnBid")]
    public Task Run(
        [QueueTrigger(QueuesModule.BidResultQueueName, Connection = "AzureWebJobsStorage")]
        string commandString, CancellationToken cancellationToken)
    {
        var result = JsonSerializer.Deserialize<UserIdDecorator<Result<Bid,Errors>?>>(commandString, _serializerOptions);
        _userContext.UserId = result!.UserId;
        _logger.LogInformation($"bid result received");
        result.Value!.Match(ok => _logger.LogInformation("bid processed successfully {Result}", ok),
            error => _logger.LogInformation("bid processed {Error}", error));
        return Task.CompletedTask;
    }
}