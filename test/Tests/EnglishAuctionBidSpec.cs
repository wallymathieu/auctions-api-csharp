using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Tests;

public class EnglishAuctionBidSpec
{
    static Bid ValidBid(Auction auction) =>
        new(User: Buyer,
            Amount: 10,
            At: auction.StartsAt.AddHours(1.0));

    static Bid BidWithSameUser(Auction auction) => ValidBid(auction) with { User = Seller };
    static Bid BidAfterAuctionEnded(Auction auction) => ValidBid(auction) with { At = auction.Expiry.AddHours(1.0) };

    static Bid BidBeforeAuctionStarted(Auction auction) =>
        ValidBid(auction) with { At = auction.StartsAt.AddHours(-1.0) };

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