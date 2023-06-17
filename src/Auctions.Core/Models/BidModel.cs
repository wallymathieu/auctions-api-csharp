using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Models;

public record BidModel(
    Amount Amount,
    string Bidder);