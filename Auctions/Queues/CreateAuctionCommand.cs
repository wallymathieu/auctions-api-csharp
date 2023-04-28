using Auctions.Domain;
using Auctions.Models;

namespace Auctions.Queues;

public class CreateAuctionCommand
{
    public UserId UserId { get; set; }
    public CreateAuctionModel Model { get; set; }
}