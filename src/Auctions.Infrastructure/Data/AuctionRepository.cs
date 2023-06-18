using Wallymathieu.Auctions.Data;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Infrastructure.Data;
/// <summary>
/// Since <see cref="AuctionRepository"/> is implemented purely in terms of <see cref="AuctionDbContext"/> it can be seen
/// as "pure" in the sense that it does not depend on or directly invoke any external code as long as you marry entity framework
/// into your domain core.
/// </summary>
public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _dbContext;

    public AuctionRepository(AuctionDbContext dbContext) => _dbContext = dbContext;

    public virtual async Task<Auction?> GetAuctionAsync(long auctionId, CancellationToken cancellationToken) =>
        await _dbContext.GetAuction(auctionId, cancellationToken);

    public virtual async Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken) =>
        await _dbContext.GetAuctionsAsync(cancellationToken);

    public async ValueTask AddAsync(Auction entity, CancellationToken cancellationToken) =>
        await _dbContext.AddAsync(entity, cancellationToken);

    public async Task<Auction?> FindAsync(object identifier, CancellationToken cancellationToken) =>
        await GetAuctionAsync((long)identifier, cancellationToken);
}