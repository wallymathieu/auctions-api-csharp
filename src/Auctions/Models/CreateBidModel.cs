using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Models;

public class CreateBidModel
{
    [Required] public Amount Amount { get; set; }
}