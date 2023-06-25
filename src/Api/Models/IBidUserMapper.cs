using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Api.Models;

public interface IBidUserMapper
{
    string? GetUserString(UserId? userId);
}