using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.DomainModels.Bids;

namespace Wallymathieu.Auctions.Models;

public static class BidMapper
{
    public static BidModel MapToBidModel(Auction auction, Bid bid, IBidUserMapper bidUserMapper)
    {
        ArgumentNullException.ThrowIfNull(auction, nameof(auction));
        ArgumentNullException.ThrowIfNull(bid, nameof(bid));
        ArgumentNullException.ThrowIfNull(bidUserMapper, nameof(bidUserMapper));

        return new BidModel(bid.Amount,
            bidUserMapper.GetUserString(bid.User),
            bid.At-auction.StartsAt);
    }

    public static BidModel MapBidToModel(Auction auction, Bid bid)
    {
        ArgumentNullException.ThrowIfNull(auction, nameof(auction));

        var bidUserMapper = auction.BidUserMapper();
        return MapToBidModel(auction, bid, bidUserMapper);
    }
}