using Auctions.Domain;

namespace Auctions.Models;

public record AuctionModel(
    long Id,
    DateTimeOffset StartsAt,
    string Title,
    DateTimeOffset Expiry,
    string User,
    CurrencyCode Currency,
    BidModel[] Bids);