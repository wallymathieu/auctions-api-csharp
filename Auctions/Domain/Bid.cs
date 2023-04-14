namespace Auctions.Domain;

public record Bid(AuctionId AuctionId, User User, Amount Amount, DateTimeOffset At)
{
    public Errors Validate(Auction auction)
    {
        var errors = Errors.None;
        if (User == auction.User) errors |= Errors.SellerCannotPlaceBids;
        if (Amount.Currency != auction.Currency) errors |= Errors.BidCurrencyConversion;
        if (At < auction.StartsAt) errors |= Errors.AuctionHasNotStarted;
        if (At > auction.Expiry) errors |= Errors.AuctionHasEnded;
        return errors;
    }
}