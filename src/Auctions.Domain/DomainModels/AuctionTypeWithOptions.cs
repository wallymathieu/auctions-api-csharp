
namespace Wallymathieu.Auctions.DomainModels;
public abstract record AuctionTypeWithOptions
{
    public static bool TryParse(string? type, out AuctionTypeWithOptions? auctionTypeWithOptions)
    {
        auctionTypeWithOptions = null;
        if (string.IsNullOrWhiteSpace(type))
            return false;

        var parts = type.Split('|');
        if (parts.Length < 1)
            return false;

        auctionTypeWithOptions = parts[0] switch
        {
            "english" when
                    parts.Length == 4
                    && long.TryParse(parts[1], out var minRaise)
                    && long.TryParse(parts[2], out var reservePrice)
                    && long.TryParse(parts[3], out var timeFrame)
                    =>
                new English(
                    MinRaise: minRaise,
                    ReservePrice: reservePrice,
                    TimeFrame: TimeSpan.FromTicks(timeFrame)),
            "blind" => new Blind(),
            "vickrey" => new Vickrey(),
            _ => null
        };
        if (auctionTypeWithOptions != null)
            return true;
        return false;
    }
    public static AuctionTypeWithOptions Parse(string type)
    {
        if (TryParse(type, out var auctionTypeWithOptions))
            return auctionTypeWithOptions!;
        throw new ArgumentException($"Invalid auction type", nameof(type));
    }
    internal record English(long MinRaise, long ReservePrice, TimeSpan TimeFrame) : AuctionTypeWithOptions
    {
    }
    internal record Vickrey : AuctionTypeWithOptions
    {
    }
    internal record Blind : AuctionTypeWithOptions
    {
    }
}

