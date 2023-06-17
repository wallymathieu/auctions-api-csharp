namespace Wallymathieu.Auctions.Domain;

public class Auction
{
#pragma warning disable CS8618
    protected Auction()
#pragma warning restore CS8618
    {

    }

    public AuctionId Id => new(AuctionId);

    public long AuctionId { get; set; }

    public DateTimeOffset StartsAt { get; init; }
    public string Title { get; init; }

    ///<summary> initial expiry </summary>
    public DateTimeOffset Expiry { get; init; }

    public UserId User { get; init; }
    public CurrencyCode Currency { get; init; }

}