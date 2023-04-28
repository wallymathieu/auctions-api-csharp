using Auctions.Domain;

namespace Auctions.Models;

public record BidModel
{
    public Amount Amount { get; init; }
    public string Bidder { get; init; }
}