using Marten;

namespace Wallymathieu.Auctions.Infrastructure.Data;
/// <summary>
/// Since <see cref="AuctionQuery"/> is implemented purely in terms of <see cref="IDocumentSession"/> it can be seen
/// as "pure" in the sense that it does not depend on or directly invoke any external code as long as you marry document
/// session into your domain core.
/// </summary>
public class AuctionQuery(IDocumentSession session) : IAuctionQuery
{
    public virtual async Task<Auction?> GetAuctionAsync(AuctionId auctionId, CancellationToken cancellationToken = default) =>
        await session.GetAuction(auctionId, cancellationToken);

    public virtual async Task<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken = default) =>
        await session.GetAuctionsAsync(cancellationToken);
}