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

    public AuctionModel MapAuctionToModel(Auction arg)
    {
        var now = _time.Now;
        var amountAndWinner = arg.TryGetAmountAndWinner(now);
        var hasEnded = arg.HasEnded(now);
        IBidUserMapper bidUserMapper = arg.OpenBidders? new BidUserMapper():new NumberedBidUserMapper(arg.Bids);
        return new AuctionModel(
            Id: arg.Id.Id,
            StartsAt :arg.StartsAt,
            Title: arg.Title,
            Expiry: arg.Expiry,
            Seller: arg.User.ToString(),
            Currency: arg.Currency,
            Price: amountAndWinner?.Amount,
            Winner: bidUserMapper.GetUserString(amountAndWinner?.Winner),
            HasEnded: hasEnded,
            Bids: arg.GetBids(now)?.Select(bid =>
                new BidModel(bid.Amount,
                    bidUserMapper.GetUserString(bid.User),
                    bid.At-arg.StartsAt)).ToArray() ?? Array.Empty<BidModel>());
    }
}