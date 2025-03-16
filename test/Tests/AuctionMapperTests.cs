using Wallymathieu.Auctions.Models;
using Wallymathieu.Auctions.Tests.Helpers;

namespace Wallymathieu.Auctions.Tests;

public class AuctionMapperTests
{
    private readonly AuctionMapper _sut;

    public AuctionMapperTests()
    {
        _sut = new AuctionMapper(new FakeSystemClock(StartsAt.AddHours(1)));
    }

    [Fact]
    public async ValueTask MapAuctionToModel_ShouldReturnExpectedModel()
    {
        // Arrange
        var auction = GetEnglishAuction();

        // Act
        var model = _sut.MapAuctionToModel(auction);

        // Assert
        Assert.NotNull(model);
        await Verify(model);
    }
}