namespace Wallymathieu.Auctions.DomainModels;

public interface IState
{
    /// <summary>
    /// increment state and add bid
    /// </summary>
    bool TryAddBid(DateTimeOffset time,Bid bid, out Errors errors);

    /// <summary>
    /// get bids for state (will return empty if in a state that does not disclose bids)
    /// </summary>
    IEnumerable<Bid> GetBids(DateTimeOffset time);

    /// <summary>
    ///try to get amount and winner, will return None if no winner
    /// </summary>
    (Amount Amount, UserId Winner)? TryGetAmountAndWinner(DateTimeOffset time);

    /// <summary>
    /// returns true if state has ended
    /// </summary>
    bool HasEnded(DateTimeOffset time);
}