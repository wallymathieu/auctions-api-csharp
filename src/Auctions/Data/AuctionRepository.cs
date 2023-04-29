using Microsoft.EntityFrameworkCore;
using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Data;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _dbContext;

    public AuctionRepository(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TimedAscendingAuction?> GetAuctionAsync(long auctionId) =>
        await _dbContext.GetAuction(auctionId);

    public async Task<IReadOnlyCollection<TimedAscendingAuction>> GetAuctionsAsync() => 
        await _dbContext.Auctions.AsNoTracking().Include(a => a.Bids).ToListAsync();
}