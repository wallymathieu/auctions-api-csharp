namespace Wallymathieu.Auctions.Functions;

public class OnAuctionHandler(
    ILoggerFactory loggerFactory,
    JsonSerializerOptions serializerOptions,
    ScopedUserContext userContext)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<OnAuctionHandler>();

    [Function("OnAuction")]
    public Task Run(
        [QueueTrigger(QueuesModule.AuctionResultQueueName, Connection = "AzureWebJobsStorage")]
        string commandString, CancellationToken cancellationToken)
    {
        var result = JsonSerializer.Deserialize<UserIdDecorator<Auction>>(commandString, serializerOptions);
        userContext.UserId = result!.UserId;
        _logger.LogInformation("Create auction result processed {Auction}", result.Value.AuctionId);
        return Task.CompletedTask;
    }
}