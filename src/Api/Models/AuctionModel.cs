using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Api.Models;
/// <summary>
/// Note that the <see cref="Seller"/> is supposed to be seller description (for instance a nick chosen by the user).
/// </summary>
public record AuctionModel(
    long Id,
    DateTimeOffset StartsAt,
    string Title,
    DateTimeOffset Expiry,
    string Seller,
    CurrencyCode Currency,
    BidModel[] Bids);