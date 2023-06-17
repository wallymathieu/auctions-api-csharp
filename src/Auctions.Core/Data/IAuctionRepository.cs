using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Data;

public interface IAuctionRepository: IRepository<Auction>
{
    Task<Auction?> GetAuctionAsync(long auctionId);
    Task<IReadOnlyCollection<Auction>> GetAuctionsAsync();
}