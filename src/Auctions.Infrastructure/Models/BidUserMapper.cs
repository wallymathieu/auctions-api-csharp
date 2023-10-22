namespace Wallymathieu.Auctions.Infrastructure.Models;

public class BidUserMapper : IBidUserMapper
{
    public string? GetUserString(UserId? userId) => userId?.ToString();
}