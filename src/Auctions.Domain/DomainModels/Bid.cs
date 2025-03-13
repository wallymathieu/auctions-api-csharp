using System.Text.Json.Serialization;

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
/// Main reason to have a separate class is to make it easier to map in Entity Framework Core. This gives us
/// another implicit dependency on Entity Framework Core.
/// <br />
/// Note that the <see cref="Bid"/> record is the same as the <see cref="BidEntity"/> class bit without the <see cref="Id"/>.
/// </summary>
public class BidEntity
{
#pragma warning disable CS8618
    private BidEntity(){}
#pragma warning restore CS8618
    public BidEntity(long id, Bid bid)
    {
        Id = id;
        ArgumentNullException.ThrowIfNull(bid);
        User = bid.User;
        Amount = bid.Amount;
        At = bid.At;
    }
    #pragma warning disable IDE0051
    [JsonConstructor]
    private BidEntity(long id, UserId user, Amount amount, DateTimeOffset at)
    #pragma warning restore IDE0051
    {
        Id = id;
        User = user;
        Amount = amount;
        At = at;
    }

    public long Id { get; init; }
    public UserId User{ get; init; }
    public Amount Amount{ get; init; }
    public DateTimeOffset At{ get; init; }

    public Bid ToBid()
    {
        return new(User, Amount, At);
    }
}