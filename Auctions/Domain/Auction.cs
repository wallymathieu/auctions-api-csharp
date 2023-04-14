namespace Auctions.Domain;

public class Auction
{
#pragma warning disable CS8618
    internal Auction()
#pragma warning restore CS8618
    {
        
    }
    public Auction(AuctionId id, DateTimeOffset startsAt, string title, DateTimeOffset expiry, User user, Currency currency)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        StartsAt = startsAt;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Expiry = expiry;
        User = user ?? throw new ArgumentNullException(nameof(user));
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    public AuctionId Id { get; init; }
    public DateTimeOffset StartsAt { get; init; }
    public string Title { get; init; }

    ///<summary> initial expiry </summary>
    public DateTimeOffset Expiry { get; set; }

    public User User { get; init; }
    public Currency Currency { get; init; }
}