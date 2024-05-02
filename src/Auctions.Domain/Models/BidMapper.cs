using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.DomainModels.Bids;

namespace Wallymathieu.Auctions.Models;

public class BidMapper
{
    public static BidModel MapToBidModel(Auction auction, Bid bid, IBidUserMapper bidUserMapper)
    {
        return new BidModel(bid.Amount,
            bidUserMapper.GetUserString(bid.User),
            bid.At-auction.StartsAt);
    }

    public static BidModel MapBidToModel(Auction auction, Bid bid)
    {
        var bidUserMapper = auction.BidUserMapper();
        return MapToBidModel(auction, bid, bidUserMapper);
    }
}