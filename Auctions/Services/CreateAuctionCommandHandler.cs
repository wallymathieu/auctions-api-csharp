using App.Data;
using Auctions.Domain;
using Auctions.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Auctions.Services;

public class CreateAuctionCommandHandler
{
    private readonly AuctionDbContext _dbContext;
    private readonly Mapper _mapper;
    private readonly IDistributedCache _cache;

    public CreateAuctionCommandHandler(AuctionDbContext dbContext, Mapper mapper, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _cache = cache;
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
        _dbContext.Auctions.Add(auction);
        await _dbContext.SaveChangesAsync();
        return _mapper.MapAuctionToModel(auction);
    }
}