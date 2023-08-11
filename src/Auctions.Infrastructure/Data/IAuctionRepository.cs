namespace Wallymathieu.Auctions.Infrastructure.Data;
/// <summary>
/// Note that DbContext implements repository and unit of work patterns.
/// The reason for this interface is not to be congruent with implementing DDD, but to allow us to add a decorator with cache logic.
/// </summary>
public interface IAuctionRepository
{
    Task<Auction?> GetAuctionAsync(long auctionId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken);
}