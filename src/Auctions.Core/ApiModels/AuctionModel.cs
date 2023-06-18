using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.ApiModels;

public record AuctionModel(
    long Id,
    DateTimeOffset StartsAt,
    string Title,
    DateTimeOffset Expiry,
    string User,
    CurrencyCode Currency,
    BidModel[] Bids);