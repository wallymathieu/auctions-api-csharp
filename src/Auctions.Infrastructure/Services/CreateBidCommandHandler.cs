using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

internal class CreateBidCommandHandler : ICreateBidCommandHandler
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly AuctionDbContext _auctionDbContext;
    private readonly IUserContext _userContext;
    private readonly ITime _time;

    public CreateBidCommandHandler(IAuctionRepository auctionRepository, AuctionDbContext auctionDbContext, IUserContext userContext, ITime time)
    {
        _auctionRepository = auctionRepository;
        _auctionDbContext = auctionDbContext;
        _userContext = userContext;
        _time = time;
    }

    public async Task<IResult<Bid, Errors>> Handle(CreateBidCommand model, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetAuctionAsync(model.AuctionId, cancellationToken);
        if (auction is null) return new Error<Bid, Errors>(Errors.UnknownAuction);
        var result = auction.TryAddBid(model, _userContext, _time);
        if (result.IsOk)
        {
            await _auctionDbContext.SaveChangesAsync(cancellationToken);
        }
        return result;
    }
}