using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Tests;

public class CanParseUserTests
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
