namespace Wallymathieu.Auctions.DomainModels;

[Flags]
public enum Errors
{
    None = 0,
    UnknownAuction = 1 << 0,
    AuctionAlreadyExists = 1 << 1,
    AuctionHasEnded = 1 << 2,
    AuctionHasNotStarted = 1 << 3,
    AuctionNotFound = 1 << 4,
    SellerCannotPlaceBids = 1 << 5,
    BidCurrencyConversion = 1 << 6,
    InvalidUserData = 1 << 7,
    MustPlaceBidOverHighestBid = 1 << 8,
    AlreadyPlacedBid = 1 << 9,
    MustRaiseWithAtLeast = 1 << 10,
    MustSpecifyAmount = 1 << 11
}