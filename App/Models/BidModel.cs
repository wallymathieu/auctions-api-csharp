using Auctions.Domain;

namespace App.Models;

public record BidModel
{
    public Amount Amount { get; init; }
    public string Bidder { get; init; }
}