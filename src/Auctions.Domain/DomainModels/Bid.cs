namespace Wallymathieu.Auctions.DomainModels;

public record Bid(UserId User, Amount Amount, DateTimeOffset At)
{
    public Errors Validate(Auction auction)
    {
        ArgumentNullException.ThrowIfNull(auction, nameof(auction));
        var errors = Errors.None;
        if (User == auction.User) errors |= Errors.SellerCannotPlaceBids;
        if (Amount.Currency != auction.Currency) errors |= Errors.BidCurrencyConversion;
        if (At < auction.StartsAt) errors |= Errors.AuctionHasNotStarted;
        if (At > auction.Expiry) errors |= Errors.AuctionHasEnded;
        return errors;
    }
}
/// <summary>
/// Main reason why we have a bid entity is to make it friendly to EF core.
/// </summary>
public class BidEntity
{
#pragma warning disable CS8618
    private BidEntity(){}
#pragma warning restore CS8618
    public BidEntity(long id, Bid bid)
    {
        ArgumentNullException.ThrowIfNull(bid);
        Id = id;
        Bid = bid;
    }
    public Bid Bid { get; init; }

    public long Id { get; init; }
}