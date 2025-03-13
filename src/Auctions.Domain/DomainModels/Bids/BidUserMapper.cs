namespace Wallymathieu.Auctions.DomainModels.Bids;

public class BidUserMapper : IBidUserMapper
{
    public string? GetUserString(UserId? userId)
    {
        return userId?.ToString();
    }
}