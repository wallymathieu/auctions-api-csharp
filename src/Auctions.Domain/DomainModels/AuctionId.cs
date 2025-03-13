using System.Globalization;

namespace Wallymathieu.Auctions.DomainModels;

[Serializable]
public record AuctionId(long Id)
{
    public override string ToString()
    {
        return Id.ToString(CultureInfo.InvariantCulture);
    }
}