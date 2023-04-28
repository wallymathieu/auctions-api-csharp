using App.Data;
using Auctions.Domain;
using Auctions.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Auctions.Services;

public class CreateBidCommandHandler
{
    private readonly AuctionDbContext _dbContext;
    private readonly ITime _time;
    private readonly IDistributedCache _cache;

    public CreateBidCommandHandler(AuctionDbContext dbContext, ITime time, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _time = time;
        _cache = cache;
    }

    public enum Result
    {
        Ok,
        NotFound,
        Error
    }
    
    public async Task<(Result Result, Errors Errors)> Handle(long auctionId, UserId userId, CreateBidModel model)
    {
        var auction = await _dbContext.GetAuction(auctionId);
        if (auction is null) return (Result.NotFound,Errors.None);
        if (auction.TryAddBid(_time.Now, 
                new Bid(userId, model.Amount, _time.Now), out var error))
        {
            await _dbContext.SaveChangesAsync();
            return (Result.Ok,Errors.None);
        }
        else
        {
            return (Result.Error,error);
        }
    }
}