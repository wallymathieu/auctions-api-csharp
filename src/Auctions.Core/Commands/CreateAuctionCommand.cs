using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Commands;

public class CreateAuctionCommand: ICommand<TimedAscendingAuction>
{
    public UserId UserId { get; set; }
    public CreateAuctionModel Model { get; set; }
}