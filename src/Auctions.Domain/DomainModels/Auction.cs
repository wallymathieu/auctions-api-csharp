using System.Text.Json.Serialization;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels.Bids;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.DomainModels;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType,
    TypeDiscriminatorPropertyName = "$type"),
JsonDerivedType(typeof(SingleSealedBidAuction), typeDiscriminator: nameof(SingleSealedBidAuction)),
JsonDerivedType(typeof(TimedAscendingAuction), typeDiscriminator: nameof(TimedAscendingAuction))]
public abstract class Auction: IEntity, IState
{
#pragma warning disable CS8618
    protected Auction()
#pragma warning restore CS8618
    {
    }

    public ICollection<BidEntity> Bids { get; init; } = new List<BidEntity>();
    public AuctionId AuctionId { get; set; }

    public DateTimeOffset StartsAt { get; init; }
    public string Title { get; init; }

    ///<summary> initial expiry </summary>
    public DateTimeOffset Expiry { get; init; }

    public UserId User { get; init; }
    public CurrencyCode Currency { get; init; }
    public AuctionType AuctionType { get; set; }

    public bool OpenBidders { get; set; } = false;

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
    public Result<Bid,Errors> TryAddBid(CreateBidCommand model, IUserContext userContext, ISystemClock systemClock)
    {
        var bid = new Bid(userContext.UserId, model.Amount, systemClock.Now);
        return TryAddBid(systemClock.Now, bid, out var error)
            ? Result<Bid, Errors>.Ok(bid)
            : Result<Bid, Errors>.Error(error);
    }

    public abstract bool TryAddBid(DateTimeOffset time, Bid bid, out Errors errors);
    public abstract IEnumerable<Bid> GetBids(DateTimeOffset time);
    public abstract (Amount Amount, UserId Winner)? TryGetAmountAndWinner(DateTimeOffset time);
    public abstract bool HasEnded(DateTimeOffset time);

    public IBidUserMapper BidUserMapper()
    {
        IBidUserMapper bidUserMapper = OpenBidders
            ? new BidUserMapper()
            : new NumberedBidUserMapper(Bids);
        return bidUserMapper;
    }
}

public enum AuctionType
{
    SingleSealedBidAuction,
    TimedAscendingAuction,
}
