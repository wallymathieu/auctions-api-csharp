using System.ComponentModel.DataAnnotations;
using Auctions.Domain;

namespace Auctions.Models;

public class CreateBidModel
{
    [Required] public Amount Amount { get; set; }
}