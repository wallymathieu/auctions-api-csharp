using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Data;

public interface IAuctionRepository: IRepository<Auction>
{
    Task<Auction?> GetAuctionAsync(long auctionId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken);
}