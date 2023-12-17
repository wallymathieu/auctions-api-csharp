using Wallymathieu.Auctions.DomainModels.Bids;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Models;

/// <summary>
/// Map to API models from domain models. We could use such a library as AutoMapper instead.
/// </summary>
public class AuctionMapper
{
    private readonly ISystemClock _systemClock;

    public AuctionMapper(ISystemClock systemClock)
    {
        _systemClock = systemClock;
    }

    public AuctionModel MapAuctionToModel(Auction auction)
    {
        var now = _systemClock.Now;
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
                MapToBidModel(auction, bid, bidUserMapper)).ToArray() ?? Array.Empty<BidModel>());
    }

    private static BidModel MapToBidModel(Auction auction, Bid bid, IBidUserMapper bidUserMapper)
    {
        return new BidModel(bid.Amount,
            bidUserMapper.GetUserString(bid.User),
            bid.At-auction.StartsAt);
    }

    public BidModel MapBidToModel(Auction auction, Bid bid)
    {
        var bidUserMapper = auction.BidUserMapper();
        return MapToBidModel(auction, bid, bidUserMapper);
    }
}