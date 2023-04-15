namespace Auctions.Domain;

public class Auction
{
#pragma warning disable CS8618
    internal Auction()
#pragma warning restore CS8618
    {
        
    }
    public Auction(DateTimeOffset startsAt, string title, DateTimeOffset expiry, UserId user, CurrencyCode currency)
    {
        StartsAt = startsAt;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Expiry = expiry;
        User = user ?? throw new ArgumentNullException(nameof(user));
        Currency = currency;
    }

    public long Id { get; set; } 
    public DateTimeOffset StartsAt { get; init; }
    public string Title { get; init; }

    ///<summary> initial expiry </summary>
    public DateTimeOffset Expiry { get; set; }

    public UserId User { get; init; }
    public CurrencyCode Currency { get; init; }
}