using Auctions.Domain;

namespace Auctions.Data;

public interface IAuctionRepository
{
    Task<TimedAscendingAuction?> GetAuctionAsync(long auctionId);
    Task<IReadOnlyCollection<TimedAscendingAuction>> GetAuctionsAsync();
}