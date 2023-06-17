using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Data;

public interface IAuctionRepository: IRepository<TimedAscendingAuction>
{
    Task<TimedAscendingAuction?> GetAuctionAsync(long auctionId);
    Task<IReadOnlyCollection<TimedAscendingAuction>> GetAuctionsAsync();
}