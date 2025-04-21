using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Commands;

public record CreateAuctionCommand(
    [Required] string Title,
    [Required] CurrencyCode Currency,
    [Required] DateTimeOffset StartsAt,
    [Required] DateTimeOffset EndsAt,
    long? MinRaise,
    long? ReservePrice,
    TimeSpan? TimeFrame,
    SingleSealedBidOptions? SingleSealedBidOptions,
    bool Open = false);