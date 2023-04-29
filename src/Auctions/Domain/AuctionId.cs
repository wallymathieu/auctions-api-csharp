using System.Globalization;

namespace Auctions.Domain;

[Serializable]
public record AuctionId(long Id)
{
    public static AuctionId NewAuctionId(long id) => new(id);
    public override string ToString() => Id.ToString(CultureInfo.InvariantCulture);
}