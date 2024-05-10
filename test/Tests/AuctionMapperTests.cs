using Wallymathieu.Auctions.Models;
using Wallymathieu.Auctions.Tests.Helpers;

namespace Wallymathieu.Auctions.Tests;

public class AuctionMapperTests
{
    private readonly AuctionMapper _sut;

    public AuctionMapperTests()
    {
        _sut = new AuctionMapper(new FakeSystemClock(DateTimeOffset.FromUnixTimeSeconds(0)));
    }

    [Fact]
    public async Task MapAuctionToModel_ShouldReturnExpectedModel()
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
