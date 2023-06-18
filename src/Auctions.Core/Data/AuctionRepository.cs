using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Data;
/// <summary>
/// Since <see cref="AuctionRepository"/> is implemented purely in terms of <see cref="IAuctionDbContext"/> it can be seen
/// as "pure" in the sense that it does not depend on or directly invoke any external code.
/// </summary>
public class AuctionRepository : IAuctionRepository
{
    private readonly IAuctionDbContext _dbContext;

    public AuctionRepository(IAuctionDbContext dbContext) => _dbContext = dbContext;

    public virtual async Task<Auction?> GetAuctionAsync(long auctionId, CancellationToken cancellationToken) =>
        await _dbContext.GetAuction(auctionId, cancellationToken);

    public virtual async Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken) =>
        await _dbContext.GetAuctionsAsync(cancellationToken);

    public ValueTask AddAsync(Auction entity, CancellationToken cancellationToken) =>
        _dbContext.AddAuctionAsync(entity, cancellationToken);

    public async Task<Auction?> FindAsync(object identifier, CancellationToken cancellationToken) =>
        await GetAuctionAsync((long)identifier, cancellationToken);
}
/// <summary>
/// Note that in the domain we don't want to know specific implementation details about the database context.
/// </summary>
public interface IAuctionDbContext
{
    Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken);
    Task<Auction?> GetAuction(long auctionId, CancellationToken cancellationToken);
    ValueTask AddAuctionAsync(Auction auction, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}