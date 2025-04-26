namespace Wallymathieu.Auctions.DomainModels.AuctionTypes;

public record EnglishAuctionType(long MinRaise, long ReservePrice, TimeSpan TimeFrame) : AuctionType
{
    public override string ToString() =>
        "English|" + MinRaise + "|" + ReservePrice + "|" + TimeFrame.Ticks;
}
