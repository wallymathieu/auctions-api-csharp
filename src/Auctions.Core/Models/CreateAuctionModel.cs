using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Models;

public record CreateAuctionModel(
    [Required] string Title,
    [Required] CurrencyCode Currency,
    [Required] DateTimeOffset StartsAt,
    [Required] DateTimeOffset EndsAt,
    long? MinRaise,
    long? ReservePrice,
    TimeSpan? TimeFrame,
    SingleSealedBidOptions? SingleSealedBidOptions);