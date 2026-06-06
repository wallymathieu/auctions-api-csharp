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
            switch (auction.AuctionType)
            {
                case AuctionType.TimedAscendingAuction:
                    Assert.IsType<TimedAscendingAuction>(deserialized);
                    Assert.Equal(((TimedAscendingAuction)auction).Options.MinRaise, ((TimedAscendingAuction)deserialized).Options.MinRaise);
                    break;
                case AuctionType.SingleSealedBidAuction:
                    Assert.IsType<SingleSealedBidAuction>(deserialized);
                    Assert.Equal(((SingleSealedBidAuction)auction).Options, ((SingleSealedBidAuction)deserialized).Options);
                    break;
            }
        });
    }

    public static TheoryData<Auction> Auctions => [
        AuctionWithBids(GetEnglishAuction()),
        AuctionWithBids(VickreyAuction)];
}