using System.ComponentModel.DataAnnotations;
using Mediator;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Commands;

public record CreateAuctionCommand(
    [Required] string Title,
    [Required] CurrencyCode Currency,
    [Required] DateTimeOffset StartsAt,
    [Required] DateTimeOffset EndsAt,
    TimedAscendingOptions? TimedAscendingOptions,
    SingleSealedBidOptions? SingleSealedBidOptions,
    bool Open = false): ICommand<Auction>;
