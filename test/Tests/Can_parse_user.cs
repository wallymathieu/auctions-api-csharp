using Auctions.Domain;

namespace Tests;

public class Can_parse_user
{
    [Fact]
    public void Buyer_or_seller()
    {
        var user = User.NewBuyerOrSeller(UserId.NewUserId(Guid.NewGuid().ToString("N")), "seller");
        Assert.Equal(user, User.TryParse(user.ToString(), out var userV) ? userV : null);
    }
    [Fact]
    public void Support()
    {
        var user = User.NewSupport(UserId.NewUserId(Guid.NewGuid().ToString("N")));
        Assert.Equal(user, User.TryParse(user.ToString(), out var userV) ? userV : null);
    }
}

public class Auction_bid
{
    Bid ValidBid(Auction auction) => 
        new(User: Buyer,
            Amount: Amount.Parse("SEK10"),
            At: auction.StartsAt.AddHours(1.0));

    Bid BidWithSameUser(Auction auction) => ValidBid(auction) with { User = Seller };
    Bid BidAfterAuctionEnded(Auction auction) => ValidBid(auction) with { At = auction.Expiry.AddHours(1.0) };
    Bid BidBeforeAuctionStarted(Auction auction) => ValidBid(auction) with { At = auction.StartsAt.AddHours(-1.0) };

    [Fact]
    public void valid_bid()
    {
        var auction = GetAuction();
        Assert.Equal(Errors.None, ValidBid(auction).Validate(auction));
    }
    
    [Fact]
    public void seller_bidding_on_auction()
    {
        var auction = GetAuction();
        Assert.Equal(Errors.SellerCannotPlaceBids, BidWithSameUser(auction).Validate(auction));
    }
    [Fact]
    public void bid_after_auction_has_ended()
    {
        var auction = GetAuction();
        Assert.Equal(Errors.AuctionHasEnded, BidAfterAuctionEnded(auction).Validate(auction));
    }
    [Fact]
    public void bid_before_auction_has_started()
    {
        var auction = GetAuction();
        Assert.Equal(Errors.AuctionHasNotStarted, BidBeforeAuctionStarted(auction).Validate(auction));
    }
}