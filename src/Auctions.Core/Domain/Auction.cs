using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Domain;

public abstract class Auction : IEntity
{
#pragma warning disable CS8618
    protected Auction()
#pragma warning restore CS8618
    {

    }

    public ICollection<BidEntity> Bids { get; init; } = new List<BidEntity>();
    public AuctionId Id => new(AuctionId);

    public long AuctionId { get; set; }

    public DateTimeOffset StartsAt { get; init; }
    public string Title { get; init; }

    ///<summary> initial expiry </summary>
    public DateTimeOffset Expiry { get; init; }

    public UserId User { get; init; }
    public CurrencyCode Currency { get; init; }
    public AuctionType AuctionType { get; set; }

    [CommandHandler]
    public static Auction Create(CreateAuctionCommand cmd)
    {
        var model = cmd.Model;
        return model.SingleSealedBidOptions!=null
            ? new SingleSealedBidAuction
                {
                    Currency = model.Currency,
                    Expiry = model.EndsAt,
                    StartsAt = model.StartsAt,
                    Title = model.Title,
                    User = cmd.UserId,
                    Options = model.SingleSealedBidOptions.Value
                }
            : new TimedAscendingAuction
                {
                    Currency = model.Currency,
                    Expiry = model.EndsAt,
                    StartsAt = model.StartsAt,
                    Title = model.Title,
                    User = cmd.UserId,
                    Options =
                    {
                        MinRaise = model.MinRaise ?? 0,
                        ReservePrice = model.ReservePrice ?? 0,
                        TimeFrame = model.TimeFrame ?? TimeSpan.Zero,
                    }
                };
    }
    [CommandHandler]
    public IResult<Bid,Errors> Handle(CreateBidCommand model, ITime time)
    {
        var bid = new Bid(model.UserId, model.Model.Amount, time.Now);
        return TryAddBid(time.Now, bid, out var error)
            ? new Ok<Bid, Errors>(bid)
            : new Error<Bid, Errors>(error);
    }

    public abstract bool TryAddBid(DateTimeOffset time, Bid bid, out Errors errors);
    public abstract IEnumerable<Bid> GetBids(DateTimeOffset time);
}

public enum AuctionType
{
    SingleSealedBidAuction,
    TimedAscendingAuction,
}