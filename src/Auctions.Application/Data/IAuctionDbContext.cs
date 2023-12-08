using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Wallymathieu.Auctions.Application.Data;
/// <summary>
/// We marry EF core in this project.
/// Adding additional unit of work and repository classes
/// does not remove the implicit dependency on EF. Here we
/// only isolate us a little from this fact in order to
/// reduce the amount of ceremony.
/// </summary>
public interface IAuctionDbContext
{
    ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Get auction with the necessary data loaded
    /// </summary>
    /// <param name="auctionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<Auction?> GetAuction(AuctionId auctionId, CancellationToken cancellationToken=default);
}