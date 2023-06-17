using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Data;

public class AuctionRepository : IAuctionRepository
{
    private readonly IAuctionDbContext _dbContext;

    public AuctionRepository(IAuctionDbContext dbContext) => _dbContext = dbContext;

    public virtual async Task<Auction?> GetAuctionAsync(long auctionId) =>
        await _dbContext.GetAuction(auctionId);

    public virtual async Task<IReadOnlyCollection<Auction>> GetAuctionsAsync() =>
        await _dbContext.GetAuctionsAsync();

    public ValueTask AddAsync(Auction entity, CancellationToken cancellationToken) =>
        _dbContext.AddAuctionAsync(entity);

    public async Task<Auction?> FindAsync(object identifier, CancellationToken cancellationToken) =>
        await GetAuctionAsync((long)identifier);
}

public interface IAuctionDbContext
{
    Task<IReadOnlyCollection<Auction>> GetAuctionsAsync();
    Task<Auction?> GetAuction(long auctionId);
    ValueTask AddAuctionAsync(Auction auction);
    Task SaveChangesAsync();
}