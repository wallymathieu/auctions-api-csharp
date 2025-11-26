using Wallymathieu.Auctions.Services;
using Wallymathieu.Auctions.Application.Data;

namespace Wallymathieu.Auctions.Application.Services;

/// <summary>
/// Glue class
/// </summary>
internal class CreateAuctionCommandHandler(
    IAuctionDbContext auctionDbContext,
    IUserContext userContext)
    : ICreateAuctionCommandHandler
{
    public async Task<Auction> Handle(CreateAuctionCommand model, CancellationToken cancellationToken = default)
    {
        var auction = Auction.Create(model, userContext);
        await auctionDbContext.AddAsync(auction, cancellationToken);
        await auctionDbContext.SaveChangesAsync(cancellationToken);

        return auction;
    }
}