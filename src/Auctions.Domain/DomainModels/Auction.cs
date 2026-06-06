using System.Text.Json.Serialization;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels.Bids;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.DomainModels;

/// <summary>
/// Represents all auction variants as one aggregate root and uses <see cref="AuctionType" /> as discriminator.
/// </summary>
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType,
     TypeDiscriminatorPropertyName = "$type"),
 JsonDerivedType(typeof(SingleSealedBidAuction), typeDiscriminator: nameof(SingleSealedBidAuction)),
 JsonDerivedType(typeof(TimedAscendingAuction), typeDiscriminator: nameof(TimedAscendingAuction))]
public class Auction : IState
{
#pragma warning disable CS8618 // Note that is used by Entity Framework Core and serialization.
    public Auction()
#pragma warning restore CS8618
    {
    }
    /// <summary>
    /// Raw list of bids. This is the storage of the bids, should not be used directly.
    /// </summary>
#pragma warning disable CA1721 // These are expected to be used internally by auction implementations.
    protected ICollection<BidEntity> Bids { get; init; } = [];
#pragma warning restore CA1721

    public AuctionId AuctionId { get; set; }

    public DateTimeOffset StartsAt { get; init; }
    public string Title { get; init; }

    ///<summary> initial expiry </summary>
    public DateTimeOffset Expiry { get; init; }

    public UserId User { get; init; }
    public CurrencyCode Currency { get; init; }
    public AuctionType AuctionType { get; init; }

    public bool OpenBidders { get; init; }
    public Guid Version { get; set; }

    /// <summary>
    /// Create an auction aggregate with shape determined by <see cref="AuctionType" />.
    /// </summary>
    public static Auction Create(CreateAuctionCommand cmd, IUserContext userContext)
    {
        ArgumentNullException.ThrowIfNull(cmd);
        ArgumentNullException.ThrowIfNull(userContext);
        if (userContext.UserId == null)
            throw new InvalidOperationException("User must be logged in to create an auction.");
        return cmd.SingleSealedBidOptions != null
            ? CreateSingleSealedBidAuction(cmd, userContext)
            : CreateTimedAscendingAuction(cmd, userContext);

        static Auction CreateSingleSealedBidAuction(CreateAuctionCommand cmd, IUserContext userContext)
        {
            return new SingleSealedBidAuction
            {
                Currency = cmd.Currency,
                Expiry = cmd.EndsAt,
                StartsAt = cmd.StartsAt,
                Title = cmd.Title,
                User = userContext.UserId!,
                Options = cmd.SingleSealedBidOptions!.Value,
                OpenBidders = cmd.Open,
                Version = Guid.NewGuid(),
            };
        }

        static Auction CreateTimedAscendingAuction(CreateAuctionCommand cmd, IUserContext userContext)
        {
            return new TimedAscendingAuction
            {
                Currency = cmd.Currency,
                Expiry = cmd.EndsAt,
                StartsAt = cmd.StartsAt,
                Title = cmd.Title,
                User = userContext.UserId!,
                Options = cmd.TimedAscendingOptions!,
                Version = Guid.NewGuid(),
                OpenBidders = cmd.Open,
            };
        }
    }

    public Result<Bid, Errors> TryAddBid(CreateBidCommand model, IUserContext userContext, ISystemClock systemClock)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(userContext);
        ArgumentNullException.ThrowIfNull(systemClock);
        if (userContext.UserId == null)
            throw new InvalidOperationException("User must be logged in to place a bid.");
        var bid = new Bid(userContext.UserId, model.Amount, systemClock.Now);
        if (TryAddBid(systemClock.Now, bid, out var error))
        {
            Version = Guid.NewGuid();
            return Result.Ok<Bid, Errors>(bid);
        }
        else
        {
            return Result.Error<Bid, Errors>(error);
        }
    }

    public bool TryAddBid(DateTimeOffset time, Bid bid, out Errors errors)
    {
        ArgumentNullException.ThrowIfNull(bid);
        return AuctionType switch
        {
            AuctionType.SingleSealedBidAuction => TryAddSingleSealedBid(time, bid, out errors),
            AuctionType.TimedAscendingAuction => TryAddTimedAscendingBid(time, bid, out errors),
            _ => throw new InvalidDataException(AuctionType.ToString())
        };
    }

    public IEnumerable<Bid> GetBids(DateTimeOffset time)
    {
        return AuctionType switch
        {
            AuctionType.SingleSealedBidAuction => GetSingleSealedBidBids(time),
            AuctionType.TimedAscendingAuction => GetTimedAscendingBids(time),
            _ => throw new InvalidDataException(AuctionType.ToString())
        };
    }

    public (long Amount, UserId Winner)? TryGetAmountAndWinner(DateTimeOffset time)
    {
        return AuctionType switch
        {
            AuctionType.SingleSealedBidAuction => TryGetSingleSealedBidAmountAndWinner(time),
            AuctionType.TimedAscendingAuction => TryGetTimedAscendingAmountAndWinner(time),
            _ => throw new InvalidDataException(AuctionType.ToString())
        };
    }

    public bool HasEnded(DateTimeOffset time)
    {
        return AuctionType switch
        {
            AuctionType.SingleSealedBidAuction => GetSingleSealedBidState(time) == SingleSealedBidState.DisclosingBids,
            AuctionType.TimedAscendingAuction => GetTimedAscendingState(time) == TimedAscendingState.HasEnded,
            _ => throw new InvalidDataException(AuctionType.ToString())
        };
    }

    public IBidUserMapper BidUserMapper()
    {
        IBidUserMapper bidUserMapper = OpenBidders
            ? new BidUserMapper()
            : new NumberedBidUserMapper(Bids);
        return bidUserMapper;
    }

    private bool TryAddSingleSealedBid(DateTimeOffset time, Bid bid, out Errors errors)
    {
        switch (GetSingleSealedBidState(time))
        {
            case SingleSealedBidState.AcceptingBids:
            {
                errors = bid.Validate(this);
                if (Bids.Any(b => b.User == bid.User))
                {
                    errors |= Errors.AlreadyPlacedBid;
                    return false;
                }

                if (errors != Errors.None) return false;
                Bids.Add(new BidEntity(0, bid));
                return true;
            }
            case SingleSealedBidState.DisclosingBids:
            {
                errors = Errors.AuctionHasEnded;
                return false;
            }
            case SingleSealedBidState.AwaitingStart:
            {
                errors = Errors.AuctionHasNotStarted;
                return false;
            }
            default:
                throw new InvalidDataException(AuctionType.ToString());
        }
    }

    private bool TryAddTimedAscendingBid(DateTimeOffset time, Bid bid, out Errors errors)
    {
        switch (GetTimedAscendingState(time))
        {
            case TimedAscendingState.OnGoing:
            {
                var timedAscendingAuction = GetTimedAscendingAuction();
                var options = timedAscendingAuction.Options;
                errors = bid.Validate(this);

                if (Bids.Count != 0)
                {
                    var maxBid = Bids.Max(b => b.Amount)!;
                    if (bid.Amount <= maxBid)
                    {
                        errors |= Errors.MustPlaceBidOverHighestBid;
                        return false;
                    }

                    if (bid.Amount < maxBid + options.MinRaise)
                    {
                        errors |= Errors.MustRaiseWithAtLeast;
                        return false;
                    }
                }

                if (errors != Errors.None) return false;

                timedAscendingAuction.EndsAt = new[] { timedAscendingAuction.EndsAt, Expiry, time + options.TimeFrame }.Where(v => v != null).Max();
                Bids.Add(new BidEntity(0, bid));
                return true;
            }
            case TimedAscendingState.HasEnded:
            {
                errors = Errors.AuctionHasEnded;
                return false;
            }
            case TimedAscendingState.AwaitingStart:
            {
                errors = Errors.AuctionHasNotStarted;
                return false;
            }
            default:
                throw new InvalidDataException(AuctionType.ToString());
        }
    }

    private IEnumerable<Bid> GetSingleSealedBidBids(DateTimeOffset time)
    {
        return GetSingleSealedBidState(time) switch
        {
            SingleSealedBidState.AcceptingBids or SingleSealedBidState.DisclosingBids => Bids.Select(b => b.ToBid()),
            _ => []
        };
    }

    private IEnumerable<Bid> GetTimedAscendingBids(DateTimeOffset time)
    {
        return GetTimedAscendingState(time) switch
        {
            TimedAscendingState.OnGoing or TimedAscendingState.HasEnded => Bids.Select(b => b.ToBid()),
            _ => []
        };
    }

    private (long Amount, UserId Winner)? TryGetSingleSealedBidAmountAndWinner(DateTimeOffset time)
    {
        if (GetSingleSealedBidState(time) != SingleSealedBidState.DisclosingBids)
            return null;
        var options = GetSingleSealedBidAuction().Options;
        return options switch
        {
            SingleSealedBidOptions.Blind when Bids.Count != 0 =>
                (Bids.MaxBy(b => b.Amount)!.Amount, Bids.MaxBy(b => b.Amount)!.User),
            SingleSealedBidOptions.Vickrey when Bids.Count >= 2 => GetVickreyWinner(),
            SingleSealedBidOptions.Vickrey when Bids.Count == 1 =>
                (Bids.Single().Amount, Bids.Single().User),
            _ => null
        };

        (long Amount, UserId Winner)? GetVickreyWinner()
        {
            var bids = Bids.OrderByDescending(b => b.Amount).Take(2).ToArray();
            return (bids[1].Amount, bids[0].User);
        }
    }

    private (long Amount, UserId Winner)? TryGetTimedAscendingAmountAndWinner(DateTimeOffset time)
    {
        if (GetTimedAscendingState(time) != TimedAscendingState.HasEnded)
            return null;
        var options = GetTimedAscendingAuction().Options;
        var winningBid = Bids.MaxBy(b => b.Amount);
        return winningBid?.Amount >= options.ReservePrice
            ? (winningBid.Amount, winningBid.User)
            : null;
    }

    private TimedAscendingAuction GetTimedAscendingAuction()
    {
        return this as TimedAscendingAuction
               ?? throw new InvalidOperationException("TimedAscendingAuction payload is required for timed ascending auctions.");
    }

    private SingleSealedBidAuction GetSingleSealedBidAuction()
    {
        return this as SingleSealedBidAuction
               ?? throw new InvalidOperationException("SingleSealedBidAuction payload is required for single sealed bid auctions.");
    }

    private SingleSealedBidState GetSingleSealedBidState(DateTimeOffset time)
    {
        return (time > StartsAt, time < Expiry) switch
        {
            (true, true) => SingleSealedBidState.AcceptingBids,
            (true, false) => SingleSealedBidState.DisclosingBids,
            (false, _) => SingleSealedBidState.AwaitingStart
        };
    }

    private TimedAscendingState GetTimedAscendingState(DateTimeOffset time)
    {
        return (time > StartsAt, time < Expiry) switch
        {
            (true, true) => TimedAscendingState.OnGoing,
            (true, false) => TimedAscendingState.HasEnded,
            (false, _) => TimedAscendingState.AwaitingStart
        };
    }

    private enum SingleSealedBidState
    {
        AwaitingStart,
        AcceptingBids,
        DisclosingBids
    }

    private enum TimedAscendingState
    {
        AwaitingStart,
        OnGoing,
        HasEnded
    }
}
/// <summary>
/// Type of auction. Discriminator used by Entity Framework Core.
/// </summary>
public enum AuctionType
{
    /// <summary>
    /// Unknown auction type.
    /// </summary>
    Unknown = -1,
    /// <summary>
    /// Single sealed bid auction.
    /// </summary>
    /// <remarks>
    ///     Single sealed bid auction is a type of auction where the bidders are not aware of the other bids. The bids are
    ///     disclosed at the end of the auction.
    ///     You can read more about the different types of blind auctions on Wikipedia:
    ///     <br />
    ///     - <a href="https://en.wikipedia.org/wiki/First-price_sealed-bid_auction">First price sealed bid auction</a> or a
    ///     <br />
    ///     - <a href="https://en.wikipedia.org/wiki/Vickrey_auction">Vickrey auction</a>
    /// </remarks>
    SingleSealedBidAuction,
    /// <summary>
    /// Timed ascending auction. Also known as an English auction.
    /// </summary>
    /// <remarks>
    ///     You can read more about this style of auction model on Wikipedia on the page about <a href="https://en.wikipedia.org/wiki/English_auction">English
    ///     auction</a>.
    /// </remarks>
    TimedAscendingAuction,
}