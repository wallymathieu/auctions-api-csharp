namespace Auctions.Domain;

public class TimedAscendingOptions
{
    ///<summary>
    /// the seller has set a minimum sale price in advance (the 'reserve' price)
    /// and the final bid does not reach that price the item remains unsold
    /// If the reserve price is 0, that is the equivalent of not setting it.
    /// </summary>
    public Amount ReservePrice { get; set; }


    ///<summary>
    /// Sometimes the auctioneer sets a minimum amount by which the next bid must exceed the current highest bid.
    /// Having min raise equal to 0 is the equivalent of not setting it.
    /// </summary>
    public Amount MinRaise { get; set; }

    ///<summary>
    /// If no competing bidder challenges the standing bid within a given time frame,
    /// the standing bid becomes the winner, and the item is sold to the highest bidder
    /// at a price equal to his or her bid.
    /// </summary>
    public TimeSpan TimeFrame { get; set; }
}