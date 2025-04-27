using System.Text.Json.Serialization;

namespace Wallymathieu.Auctions.DomainModels;

public record Bid(UserId User, long Amount, DateTimeOffset At)
{
    public Errors Validate(Auction auction)
    {
        ArgumentNullException.ThrowIfNull(auction);
        var errors = Errors.None;
        if (User == auction.User) errors |= Errors.SellerCannotPlaceBids;
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
#pragma warning disable CS8618 // Note that is used by Entity Framework Core.
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
    #pragma warning disable IDE0051 // Note the presence of JsonConstructor, i.e. we intend for this to be used by System.Text.Json.
    [JsonConstructor]
    private BidEntity(long id, UserId user, long amount, DateTimeOffset at)
    #pragma warning restore IDE0051
    {
        Id = id;
        User = user;
        Amount = amount;
        At = at;
    }

    public long Id { get; init; }
    public UserId User{ get; init; }
    public long Amount{ get; init; }
    public DateTimeOffset At{ get; init; }

    public Bid ToBid()
    {
        return new(User, Amount, At);
    }
}