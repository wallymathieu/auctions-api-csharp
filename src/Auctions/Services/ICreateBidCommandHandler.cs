using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Services;

public interface ICreateBidCommandHandler
{
    Task<(CreateBidCommandResult Result, Errors Errors)> Handle(long auctionId, UserId userId, CreateBidModel model);
}