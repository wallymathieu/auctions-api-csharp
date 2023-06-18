using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.DomainModels;

public abstract class Auction: IEntity
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
    public static Auction Create(CreateAuctionCommand cmd, IUserContext userContext)
    {
        return cmd.SingleSealedBidOptions!=null
            ? new SingleSealedBidAuction
                {
                    Currency = cmd.Currency,
                    Expiry = cmd.EndsAt,
                    StartsAt = cmd.StartsAt,
                    Title = cmd.Title,
                    User = userContext.UserId,
                    Options = cmd.SingleSealedBidOptions.Value
                }
            : new TimedAscendingAuction
                {
                    Currency = cmd.Currency,
                    Expiry = cmd.EndsAt,
                    StartsAt = cmd.StartsAt,
                    Title = cmd.Title,
                    User = userContext.UserId,
                    Options =
                    {
                        MinRaise = cmd.MinRaise ?? 0,
                        ReservePrice = cmd.ReservePrice ?? 0,
                        TimeFrame = cmd.TimeFrame ?? TimeSpan.Zero,
                    }
                };
    }
    [CommandHandler]
    public IResult<Bid,Errors> TryAddBid(CreateBidCommand model, IUserContext userContext, ITime time)
    {
        var bid = new Bid(userContext.UserId, model.Amount, time.Now);
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