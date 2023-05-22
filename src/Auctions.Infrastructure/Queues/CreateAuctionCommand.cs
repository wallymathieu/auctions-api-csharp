using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Infrastructure.Queues;

public class CreateAuctionCommand
{
    public CreateAuctionCommand(UserId userId, CreateAuctionModel model)
    {
        UserId = userId;
        Model = model;
    }

    public UserId UserId { get; set; }
    public CreateAuctionModel Model { get; set; }
}