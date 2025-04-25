using Wallymathieu.Auctions.Models.V2;
using Wallymathieu.Auctions.Tests.Helpers;

namespace Wallymathieu.Auctions.Tests.V2;

public class AuctionMapperTests
{
    private readonly AuctionMapper _sut = new(new FakeSystemClock(StartsAt.AddHours(1)));

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