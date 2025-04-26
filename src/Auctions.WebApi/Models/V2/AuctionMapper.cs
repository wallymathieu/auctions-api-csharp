using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Api.Models.V2;
/// <summary>
/// Map to API models from domain models. We could use such a library as AutoMapper instead.
/// </summary>
public class AuctionMapper(ISystemClock systemClock)
{
    public AuctionModel MapAuctionToModel(Auction auction)
    {
        ArgumentNullException.ThrowIfNull(auction);
        var now = systemClock.Now;
        var amountAndWinner = auction.TryGetAmountAndWinner(now);
        var hasEnded = auction.HasEnded(now);
        var bidUserMapper = auction.BidUserMapper();
        var bidMapper = new BidMapper(auction, bidUserMapper);
        return new AuctionModel(
            Id: auction.AuctionId.Id,
            StartsAt: auction.StartsAt,
            Title: auction.Title,
            EndsAt: auction.Expiry,
            Seller: auction.User.ToString(),
            Currency: auction.Currency,
            Price: amountAndWinner?.Amount,
            Winner: bidUserMapper.GetUserString(amountAndWinner?.Winner),
            HasEnded: hasEnded,
            Bids: auction.GetBids(now)?.Select(bid =>
                bidMapper.MapToBidModel(bid)).ToArray() ?? Array.Empty<BidModel>());
    }
}