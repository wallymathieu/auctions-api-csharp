namespace Wallymathieu.Auctions.DomainModels.AuctionTypes;

public record VickreyAuctionType : AuctionType
{
    override public string ToString() =>
        "Vickrey";
}
