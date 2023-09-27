using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Api.Models;

/// <summary>
/// Map to API models from domain models. We could use such a library as AutoMapper instead.
/// </summary>
public class AuctionMapper
{
    private readonly ITime _time;

    public AuctionMapper(ITime time)
    {
        _time = time;
    }

    public AuctionModel MapAuctionToModel(Auction auction)
    {
        var now = _time.Now;
        var amountAndWinner = auction.TryGetAmountAndWinner(now);
        var hasEnded = auction.HasEnded(now);
        var bidUserMapper = BidUserMapper(auction);
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

    private static IBidUserMapper BidUserMapper(Auction auction)
    {
        IBidUserMapper bidUserMapper = auction.OpenBidders
            ? new BidUserMapper()
            : new NumberedBidUserMapper(auction.Bids);
        return bidUserMapper;
    }

    private static BidModel MapToBidModel(Auction auction, Bid bid, IBidUserMapper bidUserMapper)
    {
        return new BidModel(bid.Amount,
            bidUserMapper.GetUserString(bid.User),
            bid.At-auction.StartsAt);
    }

    public BidModel MapBidToModel(Auction auction, Bid bid)
    {
        var bidUserMapper = BidUserMapper(auction);
        return MapToBidModel(auction, bid, bidUserMapper);
    }
}