namespace Wallymathieu.Auctions.Functions;
public class OnAuctionHandler
{
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ScopedUserContext _userContext;

    public OnAuctionHandler(ILoggerFactory loggerFactory,
        JsonSerializerOptions serializerOptions,
        ScopedUserContext userContext)
    {
        _serializerOptions = serializerOptions;
        _logger = loggerFactory.CreateLogger<OnAuctionHandler>();
        _userContext = userContext;
    }

    [Function("OnAuction")]
    public async Task Run(
        [QueueTrigger(QueuesModule.AuctionResultQueueName, Connection = "AzureWebJobsStorage")]
        string commandString, CancellationToken cancellationToken)
    {
        var result = JsonSerializer.Deserialize<UserIdDecorator<Auction>>(commandString, _serializerOptions);
        if (result == null) throw new NullReferenceException(nameof(result));
        _userContext.UserId = result.UserId;
        _logger.LogInformation("Create auction result processed {Auction}", result.Value.AuctionId);
    }
}