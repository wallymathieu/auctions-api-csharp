using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Wallymathieu.Auctions.Queues;
using Wallymathieu.Auctions.Services;

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
        string commandString)
    {
        var command = JsonSerializer.Deserialize<CreateBidCommand>(commandString, _serializerOptions);
        _logger.LogInformation($"bid received");
        if (command == null) throw new NullReferenceException(nameof(command));
        var (res,err) = await _createBidCommandHandler.Handle(command.AuctionId, command.UserId, command.Model);
        _logger.LogInformation("bid processed {Result} {Error}", res, err);
    }
}