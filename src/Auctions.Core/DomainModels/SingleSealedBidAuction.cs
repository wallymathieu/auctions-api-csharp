namespace Wallymathieu.Auctions.DomainModels;
/// <summary>
/// The responsibility of this class is to handle the domain model of "single sealed bid" auction model.
/// </summary>
public class SingleSealedBidAuction: Auction, IState
{
    public SingleSealedBidAuction()
    {
        AuctionType = AuctionType.SingleSealedBidAuction;
    }
    public SingleSealedBidOptions Options { get; init; } = new();

    private State GetState(DateTimeOffset time)
    {
        return (time > StartsAt, time < Expiry) switch
        {
            (true, true) => State.AcceptingBids,
            (true, false) => State.DisclosingBids,
            (false, _) => State.AwaitingStart
        };
    }

    private enum State
    {
        AwaitingStart,
        AcceptingBids,
        DisclosingBids,
    }

    public override bool TryAddBid(DateTimeOffset time, Bid bid, out Errors errors)
    {
        switch (GetState(time))
        {
            case State.AcceptingBids:
            {
                errors = bid.Validate(this);
                if (Bids.Any(b => b.User == bid.User))
                {
                    errors |= Errors.AlreadyPlacedBid;
                    return false;
                }

                if (errors != Errors.None) return false;

                Bids.Add(new BidEntity(0,bid.User,bid.Amount,bid.At));
                return true;
            }
            case State.DisclosingBids:
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


    public override IEnumerable<Bid> GetBids(DateTimeOffset time)
    {
        switch (GetState(time))
        {
            case State.AcceptingBids:
            case State.DisclosingBids: return Bids.Select(b=>new Bid(b.User, b.Amount, b.At));
        }

        return Array.Empty<Bid>();
    }

    public override (Amount Amount, UserId Winner)? TryGetAmountAndWinner(DateTimeOffset time)
    {
        switch (GetState(time))
        {
            case State.DisclosingBids:
            {
                switch (Options)
                {
                    case SingleSealedBidOptions.Blind when Bids.Any():
                    {
                        var winningBid = Bids.MaxBy(b => b.Amount);
                        return (winningBid!.Amount, winningBid.User);
                    }
                    case SingleSealedBidOptions.Vickrey when Bids.Count >= 2:
                    {
                        var bids = Bids.OrderByDescending(b=>b.Amount).Take(2).ToArray();
                        return (bids[1].Amount, bids[0].User);
                    }
                    case SingleSealedBidOptions.Vickrey when Bids.Count == 1:
                    {
                        var bid = Bids.Single();
                        return (bid.Amount, bid.User);
                    }
                }

                return null;
            }
            case State.AwaitingStart:
            case State.AcceptingBids:
            default: return null;
        }
    }

    public override bool HasEnded(DateTimeOffset time)
    {
        return GetState(time) switch
        {
            State.DisclosingBids => true,
            _ => false
        };
    }
}