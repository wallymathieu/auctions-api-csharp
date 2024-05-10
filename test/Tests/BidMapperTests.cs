using Wallymathieu.Auctions.Models;
using Wallymathieu.Auctions.Tests.Helpers;

namespace Wallymathieu.Auctions.Tests;

public class BidMapperTests
{
    [Fact]
    public async Task MapBidToModel_ShouldReturnExpectedModel()
    {
        // Arrange
        var clock = new FakeSystemClock(StartsAt.AddHours(1.0));
        var auction = GetEnglishAuction();
        auction.TryAddBid(clock.Now, BidOf100, out _);
        var sut = new BidMapper(auction, auction.BidUserMapper());

        // Act
        var model = sut.MapToBidModel(auction.GetBids(clock.Now).First());

        // Assert
        Assert.NotNull(model);
        await Verify(model);
    }
}