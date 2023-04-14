namespace Auctions.Domain;

public interface IState
{
    /// <summary>
    /// increment state and add bid 
    /// </summary>
    bool TryAddBid(ITime time,Bid bid, out Errors errors);

    /// <summary>
    /// get bids for state (will return empty if in a state that does not disclose bids) 
    /// </summary>
    IEnumerable<Bid> GetBids(ITime time);

    /// <summary>
    ///try to get amount and winner, will return None if no winner 
    /// </summary>
    (Amount, User)? TryGetAmountAndWinner(ITime time);

    /// <summary>
    /// returns true if state has ended
    /// </summary>
    bool HasEnded(ITime time);
}