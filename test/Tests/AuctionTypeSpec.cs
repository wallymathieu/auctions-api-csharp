using AuctionType = Wallymathieu.Auctions.DomainModels.AuctionTypes.AuctionType;

namespace Wallymathieu.Auctions.Tests;

public class AuctionTypeSpec
{
    [Theory,
     InlineData("English|1|2|3"),
     InlineData("Blind"),
     InlineData("Vickrey"),]
    public void CanParseAndHasSameToStringRepresentation(string type)
    {
        Assert.True(AuctionType.TryParse(type, out var auctionType));
        Assert.NotNull(auctionType);
        Assert.Equal(type, auctionType.ToString());
    }
}