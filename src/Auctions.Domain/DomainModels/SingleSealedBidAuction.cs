namespace Wallymathieu.Auctions.DomainModels;

public class SingleSealedBidAuction : Auction
{
    public SingleSealedBidAuction()
    {
        AuctionType = AuctionType.SingleSealedBidAuction;
    }

    public SingleSealedBidOptions Options { get; init; }
}
