namespace Auctions.Domain;

public class TimedAscendingAuction : Auction, IState
{
    public TimedAscendingOptions Options { get; init; }
    public IList<Bid> Bids { get; init; }

    private State GetState(ITime time)
    {
        return (time.Now > StartsAt, time.Now < Expiry) switch
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
        HasEnded,
    }

    public bool TryAddBid(ITime time, Bid bid, out Errors errors)
    {
        switch (GetState(time))
        {
            case State.OnGoing:
            {
                errors = bid.Validate(this);
                
                if (Bids.Any() && bid.Amount <= Bids.Max(b => b.Amount))
                {
                    errors |= Errors.MustPlaceBidOverHighestBid;
                    return false;
                }

                if (errors != Errors.None) return false;
                
                Expiry = new[] { Expiry, time.Now + Options.TimeFrame }.Max();
                Bids.Add(bid);
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
                throw new Exception();
        }
    }

    public IEnumerable<Bid> GetBids(ITime time)
    {
        switch (GetState(time))
        {
            case State.OnGoing:
            case State.HasEnded: return Bids;
        }

        return Array.Empty<Bid>();
    }

    public (Amount, User)? TryGetAmountAndWinner(ITime time)
    {
        switch (GetState(time))
        {
            case State.HasEnded:
            {
                var winningBid = Bids.MaxBy(b => b.Amount);
                return winningBid != null ? (winningBid.Amount, winningBid.User) : null;
            }
            case State.AwaitingStart:
            case State.OnGoing:
            default: return null;
        }
    }

    public bool HasEnded(ITime time)
    {
        return GetState(time) switch
        {
            State.HasEnded => true,
            _ => false
        };
    }
}