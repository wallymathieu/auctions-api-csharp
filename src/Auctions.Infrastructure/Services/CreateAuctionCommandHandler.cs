using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

internal class CreateAuctionCommandHandler:ICreateAuctionCommandHandler
{
    private readonly AuctionDbContext _auctionDbContext;
    private readonly IUserContext _userContext;

    public CreateAuctionCommandHandler(AuctionDbContext auctionDbContext, IUserContext userContext)
    {
        _auctionDbContext = auctionDbContext;
        _userContext = userContext;
    }

    public async Task<Auction> Handle(CreateAuctionCommand model, CancellationToken cancellationToken)
    {
        var auction = Auction.Create(model, _userContext);
        await _auctionDbContext.AddAsync(auction, cancellationToken);
        await _auctionDbContext.SaveChangesAsync(cancellationToken);
        return auction;
    }
}