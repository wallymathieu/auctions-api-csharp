using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Services;

public class Mapper
{
    private readonly ITime _time;

    public Mapper(ITime time)
    {
        _time = time;
    }

    public AuctionModel MapAuctionToModel(TimedAscendingAuction arg) =>
        new(arg.Id.Id, arg.StartsAt, arg.Title, arg.Expiry, arg.User.ToString(), arg.Currency, 
            arg.GetBids(_time.Now)?.Select(MapBidToModel).ToArray()??Array.Empty<BidModel>());

    public  BidModel MapBidToModel(Bid arg)
    {
        return new BidModel
        {
            Amount = arg.Amount,
            Bidder = arg.User.ToString()
        };
    }
}