using Auctions.Domain;

namespace Tests;

public class TestData
{
    public static readonly AuctionId AuctionId = new(1L);
    public static readonly string Title = "auction";
    public static DateTimeOffset StartsAt = new DateTime(2016, 1, 1);
    public static DateTimeOffset EndsAt = new DateTime(2016, 2, 1);
    public static readonly User Seller = User.NewBuyerOrSeller(new("x1"), "Seller");
    public static readonly User Buyer = User.NewBuyerOrSeller(new("x2"), "Buyer");

    public static TimedAscendingAuction GetAuction()=>
        new TimedAscendingAuction
        {
            Id = AuctionId,
            Title = Title,
            StartsAt = StartsAt,
            Expiry = EndsAt,
            User = Seller,
            Currency = new Currency(CurrencyCode.SEK),
            Bids = new List<Bid>(),
            Options = new TimedAscendingOptions
            {
                MinRaise = Sek(10),
                TimeFrame = TimeSpan.FromMinutes(1),
                ReservePrice = Sek(100),
            }
        };

    public static Amount Sek(long a) =>
        new(Value: a,
            Currency: new Currency(CurrencyCode.SEK));

    public static Bid BidOf100 =>
        new Bid(AuctionId: AuctionId,
            User: Buyer,
            Amount: Sek(100L),
            At: new DateTime(2016, 1, 2));

    public static readonly User Buyer1 = User.NewBuyerOrSeller(new("x2"), "Buyer");
    public static readonly User Buyer2 = User.NewBuyerOrSeller(new("x3"), "Buyer");

    public static Bid Bid1 => new Bid(AuctionId: AuctionId,
        User: Buyer1,
        Amount: Amount.Parse("SEK10"),
        At: StartsAt.AddHours(1.0));

    public static Bid Bid2 => new Bid(AuctionId: AuctionId,
        User: Buyer2,
        Amount: Amount.Parse("SEK12"),
        At: StartsAt.AddHours(2.0));

    public static T WithBids<T>(T state) where T:IState
    {
        var time = new TestTime();
        time.Now = Bid1.At;
        state.TryAddBid(time, Bid1, out _);
        time.Now = Bid2.At;
        state.TryAddBid(time, Bid2, out _);
        return state;
    }
}

public class TestTime:ITime
{
    public DateTimeOffset Now { get; set; }
}