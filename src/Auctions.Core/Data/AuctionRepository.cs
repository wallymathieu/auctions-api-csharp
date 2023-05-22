using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Data;

public class AuctionRepository : IAuctionRepository
{
    private readonly IAuctionDbContext _dbContext;

    public AuctionRepository(IAuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TimedAscendingAuction?> GetAuctionAsync(long auctionId) =>
        await _dbContext.GetAuction(auctionId);

    public async Task<IReadOnlyCollection<TimedAscendingAuction>> GetAuctionsAsync() =>
        await _dbContext.GetAuctionsAsync();
}

public interface IAuctionDbContext
{
    Task<IReadOnlyCollection<TimedAscendingAuction>> GetAuctionsAsync();
    Task<TimedAscendingAuction?> GetAuction(long auctionId);
    void AddAuction(TimedAscendingAuction auction);
    Task SaveChangesAsync();
}