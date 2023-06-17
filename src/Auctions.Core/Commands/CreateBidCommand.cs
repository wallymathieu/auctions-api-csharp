using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Commands;

public class CreateBidCommand: ICommand<IResult<Bid,Errors>>
{
    public CreateBidCommand(long auctionId, UserId userId, CreateBidModel model)
    {
        AuctionId = auctionId;
        UserId = userId;
        Model = model;
    }
    [Key]
    public long AuctionId { get; set; }
    public UserId UserId { get; set; }
    public CreateBidModel Model { get; set; }
}