using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Models.V1;

/// <summary>
/// Note that the <see cref="Bidder"/> is supposed to be a auction specific number or description.
/// </summary>
public record BidModel(
    Amount Amount,
    string? Bidder,
    TimeSpan At);