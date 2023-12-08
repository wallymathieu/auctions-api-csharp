namespace Wallymathieu.Auctions.Infrastructure.Data;
/// <summary>
/// Note that DbContext implements repository and unit of work patterns.
/// The reason for this interface is not to be congruent with implementing DDD, but to allow us to add a decorator with cache logic.
/// </summary>
public interface IAuctionRepository
{
    /// <summary>
    /// Returns a potentially disconnected entity
    /// </summary>
    Task<Auction?> GetAuctionAsync(AuctionId auctionId, CancellationToken cancellationToken=default);
    /// <summary>
    /// Return a list of disconnected entities
    /// </summary>
    Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken=default);
}