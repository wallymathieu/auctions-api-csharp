namespace Wallymathieu.Auctions.Application.Models;
/// <summary>
/// Note that the <see cref="Bidder"/> is supposed to be a auction specific number or description.
/// </summary>
public record BidModel(
    Amount Amount,
    string? Bidder,
    TimeSpan At);