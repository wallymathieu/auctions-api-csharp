namespace Wallymathieu.Auctions.DomainModels;

/// <summary>
///     The responsibility of this class is to handle the domain model of "timed ascending" auction model.
/// </summary>
/// <remarks>
///     You can read more about this style of auction model on Wikipedia on the page about <a href="https://en.wikipedia.org/wiki/English_auction">English
///     auction</a>.
/// </remarks>
public class TimedAscendingAuction : Auction, IState
{
    public TimedAscendingAuction()
    {
        AuctionType = AuctionType.TimedAscendingAuction;
    }

    public TimedAscendingOptions Options { get; init; } = new();

    public DateTimeOffset? EndsAt { get; set; }

    public override bool TryAddBid(DateTimeOffset time, Bid bid, out Errors errors)
    {
        ArgumentNullException.ThrowIfNull(bid);
        var state = GetState(time);
        switch (state)
        {
            case State.OnGoing:
            {
                errors = bid.Validate(this);

                if (Bids.Count != 0)
                {
                    var maxBid = Bids.Max(b => b.Amount)!;
                    if (bid.Amount <= maxBid)
                    {
                        errors |= Errors.MustPlaceBidOverHighestBid;
                        return false;
                    }

                    if (bid.Amount.Value < maxBid.Value + Options.MinRaise)
                    {
                        errors |= Errors.MustRaiseWithAtLeast;
                        return false;
                    }
                }

                if (errors != Errors.None) return false;

                EndsAt = new[] { EndsAt, Expiry, time + Options.TimeFrame }.Where(v => v != null).Max();
                Bids.Add(new BidEntity(0, bid));
                return true;
            }
            case State.HasEnded:
            {
                errors = Errors.AuctionHasEnded;
                return false;
            }
            case State.AwaitingStart:
            {
                errors = Errors.AuctionHasNotStarted;
                return false;
            }
            default:
                throw new InvalidDataException(state.ToString());
        }
    }

    public override IEnumerable<Bid> GetBids(DateTimeOffset time)
    {
        switch (GetState(time))
        {
            case State.OnGoing:
            case State.HasEnded: return Bids.Select(b => b.ToBid());
        }

        return Array.Empty<Bid>();
    }

    public override (Amount Amount, UserId Winner)? TryGetAmountAndWinner(DateTimeOffset time)
    {
        switch (GetState(time))
        {
            case State.HasEnded:
            {
                var winningBid = Bids.MaxBy(b => b.Amount);
                return winningBid?.Amount.Value >= Options.ReservePrice
                    ? (winningBid.Amount, winningBid.User)
                    : null;
            }
            case State.AwaitingStart:
            case State.OnGoing:
            default: return null;
        }
    }

    public override bool HasEnded(DateTimeOffset time)
    {
        return GetState(time) switch
        {
            State.HasEnded => true,
            _ => false
        };
    }

    private State GetState(DateTimeOffset time)
    {
        return (time > StartsAt, time < Expiry) switch
        {
            (true, true) => State.OnGoing,
            (true, false) => State.HasEnded,
            (false, _) => State.AwaitingStart
        };
    }

    private enum State
    {
        AwaitingStart,
        OnGoing,
        HasEnded
    }
}