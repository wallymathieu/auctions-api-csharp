namespace Wallymathieu.Auctions.DomainModels.AuctionTypes;

public abstract record AuctionType
{
    public static bool TryParse(string? type, out AuctionType? auctionTypeWithOptions)
    {
        auctionTypeWithOptions = null;
        if (string.IsNullOrWhiteSpace(type))
            return false;

        var parts = type.Split('|');
        if (parts.Length < 1)
            return false;

        auctionTypeWithOptions = parts[0] switch
        {
            "English" when
                    parts.Length == 4
                    && long.TryParse(parts[1], out var minRaise)
                    && long.TryParse(parts[2], out var reservePrice)
                    && long.TryParse(parts[3], out var timeFrame)
                    =>
                new EnglishAuctionType(
                    MinRaise: minRaise,
                    ReservePrice: reservePrice,
                    TimeFrame: TimeSpan.FromTicks(timeFrame)),
            "Blind" => new BlindAuctionType(),
            "Vickrey" => new VickreyAuctionType(),
            _ => null
        };
        return auctionTypeWithOptions != null;
    }
    public static AuctionType Parse(string type)
    {
        if (TryParse(type, out var auctionTypeWithOptions))
            return auctionTypeWithOptions!;
        throw new ArgumentException($"Invalid auction type", nameof(type));
    }
}
