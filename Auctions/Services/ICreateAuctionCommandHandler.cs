using Auctions.Domain;
using Auctions.Models;

namespace Auctions.Services;

public interface ICreateAuctionCommandHandler
{
    Task<AuctionModel> Handle(UserId userId, CreateAuctionModel model);
}