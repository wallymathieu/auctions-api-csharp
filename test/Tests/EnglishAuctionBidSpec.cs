using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Tests;

public class EnglishAuctionBidSpec
{
    private static Bid ValidBid(Auction auction)
    {
        return new Bid(Buyer,
            Amount.Parse("SEK10"),
            auction.StartsAt.AddHours(1.0));
    }

    private static Bid BidWithSameUser(Auction auction)
    {
        return ValidBid(auction) with { User = Seller };
    }

    private static Bid BidAfterAuctionEnded(Auction auction)
    {
        return ValidBid(auction) with { At = auction.Expiry.AddHours(1.0) };
    }

    private static Bid BidBeforeAuctionStarted(Auction auction)
    {
        return ValidBid(auction) with { At = auction.StartsAt.AddHours(-1.0) };
    }

    [Fact]
    public void valid_bid()
    {
        var auction = GetEnglishAuction();
        Assert.Equal(Errors.None, ValidBid(auction).Validate(auction));
    }

    [Fact]
    public void seller_bidding_on_auction()
    {
        var auction = GetEnglishAuction();
        Assert.Equal(Errors.SellerCannotPlaceBids, BidWithSameUser(auction).Validate(auction));
    }

    [Fact]
    public void bid_after_auction_has_ended()
    {
        var auction = GetEnglishAuction();
        Assert.Equal(Errors.AuctionHasEnded, BidAfterAuctionEnded(auction).Validate(auction));
    }

    [Fact]
    public void bid_before_auction_has_started()
    {
        var auction = GetEnglishAuction();
        Assert.Equal(Errors.AuctionHasNotStarted, BidBeforeAuctionStarted(auction).Validate(auction));
    }
}