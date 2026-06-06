using System.Text.Json;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Tests;

public class DomainModelSerializationSpec
{
    [Theory, MemberData(nameof(Auctions))]
    public void Can_serialize_and_deserialize_discriminated_model(Auction auction)
    {
        var auctionsJson = JsonSerializer.Serialize(auction);
        var deserialized = JsonSerializer.Deserialize<Auction>(auctionsJson);
        Assert.Multiple(() =>
        {
            Assert.NotNull(deserialized);
            Assert.Equal(auction.AuctionType, deserialized.AuctionType);
            Assert.Equal(
                (auction as TimedAscendingAuction)?.Options.MinRaise,
                (deserialized as TimedAscendingAuction)?.Options.MinRaise);
            Assert.Equal(
                (auction as SingleSealedBidAuction)?.Options,
                (deserialized as SingleSealedBidAuction)?.Options);
        });
    }

    public static TheoryData<Auction> Auctions => [
        AuctionWithBids(GetEnglishAuction()),
        AuctionWithBids(VickreyAuction)];
}