using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Services;

public interface ICreateAuctionCommandHandler
{
    Task<AuctionModel> Handle(UserId userId, CreateAuctionModel model);
}