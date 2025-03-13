namespace Wallymathieu.Auctions.DomainModels;

public enum SingleSealedBidOptions
{
    /// Sealed first-price auction
    /// In this type of auction all bidders simultaneously submit sealed bids so that no bidder knows the bid of any
    /// other participant. The highest bidder pays the price they submitted.
    /// This type of auction is distinct from the English auction, in that bidders can only submit one bid each.
    Blind,

    /// Also known as a sealed-bid second-price auction.
    /// This is identical to the sealed first-price auction except that the winning bidder pays the second-highest bid
    /// rather than his or her own
    Vickrey,
}
