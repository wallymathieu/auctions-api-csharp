using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Models;

/// <summary>
/// Map to API models from domain models. We could use such a library as AutoMapper instead.
/// </summary>
public class AuctionMapper(ISystemClock systemClock)
{
    public AuctionModel MapAuctionToModel(Auction auction)
    {
        ArgumentNullException.ThrowIfNull(auction, nameof(auction));
        var now = systemClock.Now;
        var amountAndWinner = auction.TryGetAmountAndWinner(now);
        var hasEnded = auction.HasEnded(now);
        var bidUserMapper = auction.BidUserMapper();
        return new AuctionModel(
            Id: auction.AuctionId.Id,
            StartsAt :auction.StartsAt,
            Title: auction.Title,
            Expiry: auction.Expiry,
            Seller: auction.User.ToString(),
            Currency: auction.Currency,
            Price: amountAndWinner?.Amount,
            Winner: bidUserMapper.GetUserString(amountAndWinner?.Winner),
            HasEnded: hasEnded,
            Bids: auction.GetBids(now)?.Select(bid =>
                BidMapper.MapToBidModel(auction, bid, bidUserMapper)).ToArray() ?? Array.Empty<BidModel>());
    }
}
