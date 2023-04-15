namespace Auctions.Domain;

public class TimedAscendingAuction : Auction, IState
{
    public TimedAscendingOptions Options { get; init; } = new();
    public IList<BidEntity> Bids { get; init; } = new List<BidEntity>();

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
        HasEnded,
    }

    public bool TryAddBid(DateTimeOffset time, Bid bid, out Errors errors)
    {
        switch (GetState(time))
        {
            case State.OnGoing:
            {
                errors = bid.Validate(this);
                
                if (Bids.Any())
                {
                    var maxBid = Bids.Max(b => b.Amount)!;
                    if (bid.Amount <= maxBid)
                    {
                       errors |= Errors.MustPlaceBidOverHighestBid;
                       return false;
                    }
                    if (bid.Amount < maxBid+Options.MinRaise)
                    {
                        errors |= Errors.MustRaiseWithAtLeast;
                        return false;
                    }
                }

                if (errors != Errors.None) return false;
                
                Expiry = new[] { Expiry, time + Options.TimeFrame }.Max();
                Bids.Add(new BidEntity(0,bid.AuctionId,bid.User,bid.Amount,bid.At));
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

    public IEnumerable<Bid> GetBids(DateTimeOffset time)
    {
        switch (GetState(time))
        {
            case State.OnGoing:
            case State.HasEnded: return Bids.Select(b=>new Bid(b.AuctionId,b.User,b.Amount,b.At));
        }

        return Array.Empty<Bid>();
    }

    public (Amount, UserId)? TryGetAmountAndWinner(DateTimeOffset time)
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

    public bool HasEnded(DateTimeOffset time)
    {
        return GetState(time) switch
        {
            State.HasEnded => true,
            _ => false
        };
    }
}