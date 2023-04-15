using Auctions.Domain;

namespace App.Models;

public class CreateAuctionModel
{
    public CurrencyCode Currency { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public string Title { get; set; }
}