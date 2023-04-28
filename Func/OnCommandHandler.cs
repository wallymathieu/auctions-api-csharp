using Auctions.Domain;
using Auctions.Models;
using Auctions.Queues;
using Auctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Func;

public static class OnCommandHandler
{
    [FunctionName("OnAuctionCommand")]
    [QueueOutput(QueuesModule.AuctionResultQueueName)]
    public static async Task<AuctionModel> RunAuctionCommandAsync(
        [Microsoft.Azure.WebJobs.QueueTrigger(QueuesModule.AuctionCommandQueueName, Connection = "")] CreateAuctionCommand myQueueItem,
        ILogger log,
        ICreateAuctionCommandHandler createAuctionCommandHandler)
    {
        log.LogInformation("Create auction received");
        var result = await createAuctionCommandHandler.Handle(myQueueItem.UserId, myQueueItem.Model);

        return result;
    }
    
    [QueueOutput(QueuesModule.BidResultQueueName)]
    [FunctionName("OnBidCommand")]
    public static async Task<(CreateBidCommandResult Result, Errors Errors)> RunBidCommandAsync(
        [Microsoft.Azure.WebJobs.QueueTrigger(QueuesModule.BidCommandQueueName, Connection = "")] CreateBidCommand myQueueItem, ILogger log,
        ICreateBidCommandHandler createBidCommandHandler)
    {
        log.LogInformation($"bid received");
        var res= await createBidCommandHandler.Handle(myQueueItem.AuctionId, myQueueItem.UserId, myQueueItem.Model);
        return res;
    }
}