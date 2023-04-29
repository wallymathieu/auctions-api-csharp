using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Queues;

public class CreateBidCommand
{
    public CreateBidCommand(long auctionId, UserId userId, CreateBidModel model)
    {
        AuctionId = auctionId;
        UserId = userId;
        Model = model;
    }

    public long AuctionId { get; set; }
    public UserId UserId { get; set; }
    public CreateBidModel Model { get; set; }
}