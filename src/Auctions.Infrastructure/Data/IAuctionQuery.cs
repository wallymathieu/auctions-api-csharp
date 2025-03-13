namespace Wallymathieu.Auctions.Infrastructure.Data;

/// <summary>
///     The reason for this interface is to allow us to add a decorator with cache logic.
/// </summary>
public interface IAuctionQuery
{
    /// <summary>
    ///     Returns a potentially disconnected entity
    /// </summary>
    Task<Auction?> GetAuctionAsync(AuctionId auctionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Return a list of disconnected entities
    /// </summary>
    Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken = default);
}