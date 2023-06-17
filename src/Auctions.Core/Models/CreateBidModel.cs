using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Models;

public record CreateBidModel([Required] Amount Amount);