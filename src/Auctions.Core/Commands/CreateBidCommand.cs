using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Commands;

public record CreateBidCommand(Amount Amount,[property:Key] long AuctionId) : ICommand<IResult<Bid, Errors>>
{
}