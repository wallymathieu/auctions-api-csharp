using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Queues;

public class CreateAuctionCommand
{
    public UserId UserId { get; set; }
    public CreateAuctionModel Model { get; set; }
}