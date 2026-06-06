namespace Wallymathieu.Auctions.DomainModels;

public class TimedAscendingAuction : Auction
{
    public TimedAscendingAuction()
    {
        AuctionType = AuctionType.TimedAscendingAuction;
    }

    public TimedAscendingOptions Options { get; init; } = new();

    public DateTimeOffset? EndsAt { get; set; }
}
