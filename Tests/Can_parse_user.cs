using Auctions.Domain;

namespace Tests;

public class Can_parse_user
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