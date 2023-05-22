using Wallymathieu.Auctions.Data;
using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Services;

internal class CreateBidCommandHandler : ICreateBidCommandHandler
{
    private readonly IAuctionDbContext _dbContext;
    private readonly ITime _time;

    public CreateBidCommandHandler(IAuctionDbContext dbContext, ITime time)
    {
        _dbContext = dbContext;
        _time = time;
    }

    public async Task<(CreateBidCommandResult Result, Errors Errors)> Handle(long auctionId, UserId userId, CreateBidModel model)
    {
        var auction = await _dbContext.GetAuction(auctionId);
        if (auction is null) return (CreateBidCommandResult.NotFound,Errors.None);
        if (auction.TryAddBid(_time.Now,
                new Bid(userId, model.Amount, _time.Now), out var error))
        {
            await _dbContext.SaveChangesAsync();
            return (CreateBidCommandResult.Ok,Errors.None);
        }
        else
        {
            return (CreateBidCommandResult.Error,error);
        }
    }
}