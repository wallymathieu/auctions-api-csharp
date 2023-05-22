using Wallymathieu.Auctions.Data;
using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Services;

internal class CreateAuctionCommandHandler : ICreateAuctionCommandHandler
{
    private readonly IAuctionDbContext _dbContext;
    private readonly Mapper _mapper;

    public CreateAuctionCommandHandler(IAuctionDbContext dbContext, Mapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<AuctionModel> Handle(UserId userId, CreateAuctionModel model)
    {
        var auction = new TimedAscendingAuction
        {
            Currency = model.Currency,
            Expiry = model.EndsAt,
            StartsAt = model.StartsAt,
            Title = model.Title,
            User = userId,
            Options =
            {
                MinRaise = model.MinRaise??0,
                ReservePrice = model.ReservePrice??0,
                TimeFrame = model.TimeFrame?? TimeSpan.Zero,
            }
        };
        _dbContext.AddAuction(auction);
        await _dbContext.SaveChangesAsync();
        return _mapper.MapAuctionToModel(auction);
    }
}