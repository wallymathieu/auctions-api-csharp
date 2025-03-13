using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Tests;

public class EnglishAuctionStateSpec
{
    private static TimedAscendingAuction GetState()
    {
        return WithBids(GetEnglishAuction());
    }

    [Fact]
    public void bid_after_auction_has_ended()
    {
        Assert.Equal(Errors.AuctionHasEnded,
            !GetState().TryAddBid(EndsAt, BidOf100, out var err) ? err : default);
    }

    [Fact]
    public void english_auction_winner_and_price()
    {
        var maybeAmountAndWinner = GetState().TryGetAmountAndWinner(EndsAt);
        Assert.Equal((Bid2.Amount, Bid2.User), maybeAmountAndWinner);
    }

    [Fact]
    public void english_auction_Cant_place_bid_lower_than_highest_bid()
    {
        var auction = GetEnglishAuction();
        var at = StartsAt.AddHours(1);
        Assert.True(auction.TryAddBid(at, BidOf100 with { At = at }, out _));
        at = StartsAt.AddHours(2);
        Assert.False(auction.TryAddBid(at, BidOf100 with { At = at }, out var err));
        Assert.Equal(Errors.MustPlaceBidOverHighestBid, err);
    }
}