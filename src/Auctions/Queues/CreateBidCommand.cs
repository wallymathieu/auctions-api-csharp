using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Queues;

public class CreateBidCommand
{
    public long AuctionId { get; set; }
    public UserId UserId { get; set; }
    public CreateBidModel Model { get; set; }
}