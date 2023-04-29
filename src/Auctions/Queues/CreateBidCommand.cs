using Auctions.Domain;
using Auctions.Models;

namespace Auctions.Queues;

public class CreateBidCommand
{
    public long AuctionId { get; set; }
    public UserId UserId { get; set; }
    public CreateBidModel Model { get; set; }
}