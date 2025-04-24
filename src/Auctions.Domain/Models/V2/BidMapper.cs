using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.DomainModels.Bids;

namespace Wallymathieu.Auctions.Models.V2;

public class BidMapper(Auction auction, IBidUserMapper bidUserMapper)
{
    public BidModel MapToBidModel(Bid bid)
    {
        ArgumentNullException.ThrowIfNull(bid);

        return new BidModel(bid.Amount,
            bidUserMapper.GetUserString(bid.User),
            bid.At - auction.StartsAt);
    }
}