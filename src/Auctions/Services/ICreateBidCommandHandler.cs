using Auctions.Domain;
using Auctions.Models;

namespace Auctions.Services;

public interface ICreateBidCommandHandler
{
    Task<(CreateBidCommandResult Result, Errors Errors)> Handle(long auctionId, UserId userId, CreateBidModel model);
}