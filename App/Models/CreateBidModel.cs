using System.ComponentModel.DataAnnotations;
using Auctions.Domain;

namespace App.Models;

public class CreateBidModel
{
    [Required] public Amount Amount { get; set; }
}