using System;
using Auctions.Queues;
using Auctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Auctions.Functions
{
    public class OnAuctionCommandHandler
    {
        private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
        private readonly ILogger _logger;

        public OnAuctionCommandHandler(ILoggerFactory loggerFactory,
            ICreateAuctionCommandHandler createAuctionCommandHandler)
        {
            _createAuctionCommandHandler = createAuctionCommandHandler;
            _logger = loggerFactory.CreateLogger<OnAuctionCommandHandler>();
        }

        [Function("OnAuctionCommand")]
        public async Task Run(
            [QueueTrigger(QueuesModule.AuctionCommandQueueName, Connection = "AzureWebJobsStorage")]
            CreateAuctionCommand myQueueItem)
        {
            _logger.LogInformation("Create auction received");
            var result = await _createAuctionCommandHandler.Handle(myQueueItem.UserId, myQueueItem.Model);
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}