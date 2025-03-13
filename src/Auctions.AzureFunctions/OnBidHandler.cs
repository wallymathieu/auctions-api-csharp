namespace Wallymathieu.Auctions.Functions;

public class OnBidHandler(
    ILoggerFactory loggerFactory,
    JsonSerializerOptions serializerOptions,
    ScopedUserContext userContext)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<OnBidHandler>();

    [Function("OnBid")]
    public Task Run(
        [QueueTrigger(QueuesModule.BidResultQueueName, Connection = "AzureWebJobsStorage")]
        string commandString, CancellationToken cancellationToken)
    {
        var result =
            JsonSerializer.Deserialize<UserIdDecorator<Result<Bid, Errors>?>>(commandString, serializerOptions);
        userContext.UserId = result!.UserId;
        _logger.LogInformation("bid result received");
        result.Value!.Match(ok => _logger.LogInformation("bid processed successfully {Result}", ok),
            error => _logger.LogInformation("bid processed {Error}", error));
        return Task.CompletedTask;
    }
}