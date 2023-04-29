using System.ComponentModel.DataAnnotations;
using Auctions.Domain;

namespace Auctions.Models;

public class CreateAuctionModel
{
    [Required] public CurrencyCode Currency { get; set; }
    [Required] public DateTimeOffset StartsAt { get; set; }
    [Required] public DateTimeOffset EndsAt { get; set; }
    [Required] public string Title { get; set; }
    public long? MinRaise { get; set; }
    public long? ReservePrice { get; set; }
    public TimeSpan? TimeFrame { get; set; }
}