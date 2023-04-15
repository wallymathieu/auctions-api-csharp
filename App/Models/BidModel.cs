namespace App.Models;

public record BidModel
{
    public string Amount { get; init; }
    public string Bidder { get; init; }
}