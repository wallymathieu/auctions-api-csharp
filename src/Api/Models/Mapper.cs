using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Api.Models;
/// <summary>
/// Map to API models from domain models. We could use such a library as AutoMapper instead.
/// </summary>
public class Mapper
{
    private readonly ITime _time;

    public Mapper(ITime time)
    {
        _time = time;
    }

    public AuctionModel MapAuctionToModel(Auction arg) =>
        new(arg.Id.Id, arg.StartsAt, arg.Title, arg.Expiry, arg.User.ToString(), arg.Currency,
            arg.GetBids(_time.Now)?.Select(MapBidToModel).ToArray()??Array.Empty<BidModel>());

    public BidModel MapBidToModel(Bid arg) =>
        new(arg.Amount, arg.User.ToString());
}
