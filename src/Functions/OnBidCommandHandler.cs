using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Wallymathieu.Auctions.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Functions;

public class OnBidCommandHandler
{
    private readonly ICreateBidCommandHandler _createBidCommandHandler;
    private readonly ILogger _logger;

    public OnBidCommandHandler(ILoggerFactory loggerFactory, ICreateBidCommandHandler createBidCommandHandler)
    {
        _createBidCommandHandler = createBidCommandHandler;

        _logger = loggerFactory.CreateLogger<OnBidCommandHandler>();
    }

    [Function("OnBidCommand")]
    public async Task Run(
        [QueueTrigger(QueuesModule.BidCommandQueueName, Connection = "AzureWebJobsStorage")]
        CreateBidCommand myQueueItem)
    {
        _logger.LogInformation($"bid received");
        var res = await _createBidCommandHandler.Handle(myQueueItem.AuctionId, myQueueItem.UserId, myQueueItem.Model);
    }
}