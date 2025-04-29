using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Api.Models.V2;

/// <summary>
/// Note that the <see cref="Seller"/> is supposed to be seller description (for instance a nick chosen by the user).
/// </summary>
public record AuctionModel(
    long Id,
    DateTimeOffset StartsAt,
    string Title,
    DateTimeOffset EndsAt,
    string? Seller,
    CurrencyCode Currency,
#pragma warning disable CA1819 // Properties should not return arrays
    BidModel[] Bids,
#pragma warning restore CA1819 // Properties should not return arrays
    long? Price,
    string? Winner,
    bool HasEnded);
