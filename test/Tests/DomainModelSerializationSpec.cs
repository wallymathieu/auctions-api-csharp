using System.Text.Json;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Tests;

public class DomainModelSerializationSpec
{
    [Theory, MemberData(nameof(Auctions))]
    public void Can_serialize_and_deserialize_polymorphic_model(Auction auction)
    {
        var auctionsJson = JsonSerializer.Serialize(auction);
        var deserialized = JsonSerializer.Deserialize<Auction>(auctionsJson);
        Assert.Multiple(() =>
        {
            Assert.NotNull(deserialized);
            Assert.Equal(auction.GetType(), deserialized.GetType());
        });
    }

    public static IEnumerable<object[]> Auctions => new List<object[]>()
    {
        new object[] { WithBids(GetEnglishAuction()) },
        new object[] { WithBids(VickreyAuction) }
    };
}