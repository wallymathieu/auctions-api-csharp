using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Data;
/// <summary>
/// Note that in the domain we don't want to know specific implementation details about
/// the database context (concrete details) in order to be in line with Dependency Inversion Principle.
/// </summary>
public interface IAuctionRepository: IRepository<Auction>
{
    Task<Auction?> GetAuctionAsync(long auctionId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken);
}