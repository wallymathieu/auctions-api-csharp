using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Commands;

public class CreateAuctionCommand: ICommand<Auction>
{
    public CreateAuctionCommand(UserId userId, CreateAuctionModel model)
    {
        UserId = userId;
        Model = model;
    }

    public UserId UserId { get; }
    public CreateAuctionModel Model { get; }
}