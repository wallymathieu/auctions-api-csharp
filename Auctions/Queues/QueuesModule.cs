namespace Auctions.Queues;

public class QueuesModule
{
    public const string AuctionCommandQueueName = "auctions-commands";
    public const string BidCommandQueueName = "bids-commands";
    public const string AuctionResultQueueName = "auctions-results";
    public const string BidResultQueueName = "bids-results";
}