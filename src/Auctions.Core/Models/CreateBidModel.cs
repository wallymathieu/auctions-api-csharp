using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Models;

public class CreateBidModel
{
    [Required] public Amount Amount { get; set; }
}