using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Data;

public class AuctionRepository : IAuctionRepository
{
    private readonly IAuctionDbContext _dbContext;

    public AuctionRepository(IAuctionDbContext dbContext) => _dbContext = dbContext;

    public virtual async Task<TimedAscendingAuction?> GetAuctionAsync(long auctionId) =>
        await _dbContext.GetAuction(auctionId);

    public virtual async Task<IReadOnlyCollection<TimedAscendingAuction>> GetAuctionsAsync() =>
        await _dbContext.GetAuctionsAsync();

    public ValueTask AddAsync(TimedAscendingAuction entity, CancellationToken cancellationToken) =>
        _dbContext.AddAuctionAsync(entity);

    public async Task<TimedAscendingAuction?> FindAsync(object identifier, CancellationToken cancellationToken) =>
        await GetAuctionAsync((long)identifier);
}

public interface IAuctionDbContext
{
    Task<IReadOnlyCollection<TimedAscendingAuction>> GetAuctionsAsync();
    Task<TimedAscendingAuction?> GetAuction(long auctionId);
    ValueTask AddAuctionAsync(TimedAscendingAuction auction);
    Task SaveChangesAsync();
}