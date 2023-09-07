using System.Globalization;

namespace Wallymathieu.Auctions.DomainModels;

[Serializable]
public record AuctionId(long Id)
{
    public override string ToString() => Id.ToString(CultureInfo.InvariantCulture);
}