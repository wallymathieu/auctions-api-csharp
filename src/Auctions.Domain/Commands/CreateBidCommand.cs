using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Commands;

public record CreateBidCommand(long Amount,[property:Key] AuctionId AuctionId) : ICommand<Result<Bid, Errors>>
{
}
