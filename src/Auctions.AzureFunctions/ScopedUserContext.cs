using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Functions;

public class ScopedUserContext : IUserContext
{
    public UserId? UserId { get; set; }
}