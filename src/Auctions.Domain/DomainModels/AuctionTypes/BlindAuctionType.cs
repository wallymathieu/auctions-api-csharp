namespace Wallymathieu.Auctions.DomainModels.AuctionTypes;
public record BlindAuctionType : AuctionType
{
    override public string ToString() =>
        "Blind";
}
