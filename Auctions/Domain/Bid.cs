namespace Auctions.Domain;

public record Bid(UserId User, Amount Amount, DateTimeOffset At)
{
    public Errors Validate(Auction auction)
    {
        var errors = Errors.None;
        if (User == auction.User) errors |= Errors.SellerCannotPlaceBids;
        if (Amount.Currency != auction.Currency) errors |= Errors.BidCurrencyConversion;
        if (At < auction.StartsAt) errors |= Errors.AuctionHasNotStarted;
        if (At > auction.Expiry) errors |= Errors.AuctionHasEnded;
        return errors;
    }
}
public class BidEntity
{
#pragma warning disable CS8618
    private BidEntity(){}
#pragma warning restore CS8618
    public BidEntity(long id, UserId user, Amount amount, DateTimeOffset at)
    {
        Id = id;
        User = user ?? throw new ArgumentNullException(nameof(user));
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        At = at;
    }

    public long Id { get; init; }
    public UserId User{ get; init; }
    public Amount Amount{ get; init; }
    public DateTimeOffset At{ get; init; }
}