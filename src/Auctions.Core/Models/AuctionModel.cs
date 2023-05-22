using Wallymathieu.Auctions.Domain;

namespace Wallymathieu.Auctions.Models;

public record AuctionModel(
    long Id,
    DateTimeOffset StartsAt,
    string Title,
    DateTimeOffset Expiry,
    string User,
    CurrencyCode Currency,
    BidModel[] Bids);