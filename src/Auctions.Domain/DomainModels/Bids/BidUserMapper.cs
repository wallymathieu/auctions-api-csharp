namespace Wallymathieu.Auctions.DomainModels.Bids;

public class BidUserMapper : IBidUserMapper
{
    public string? GetUserString(UserId? userId) => userId?.ToString();
}