using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Api.Models;

public class BidUserMapper : IBidUserMapper
{
    public string? GetUserString(UserId? userId) => userId?.ToString();
}