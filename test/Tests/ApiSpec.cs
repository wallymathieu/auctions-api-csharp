using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wallymathieu.Auctions.Tests.Helpers;
using Wallymathieu.Auctions.Tests.Helpers.MsSql;
using Wallymathieu.Auctions.Tests.Helpers.Sqlite;

namespace Wallymathieu.Auctions.Tests;
using static JsonSamples;
using static JsonHelper;

public class JwtAuthAndSqlLiteApiFixture():
    ApiFixture(new SqliteDatabaseFixture(), new JwtApiAuth());
public class MsClientAuthAndSqlLiteApiFixture():
    ApiFixture(new SqliteDatabaseFixture(), new JwtApiAuth());
public class JwtAuthAndMsSqlApiFixture():
    ApiFixture(new MsSqlDatabaseFixture(), new JwtApiAuth());
public class MsClientAuthAndMsSqlApiFixture():
    ApiFixture(new MsSqlDatabaseFixture(), new JwtApiAuth());

[Collection("Sqlite")]
public class ApiSyncSpecJwtTokenSqlLite(JwtAuthAndSqlLiteApiFixture fixture):
    ApiSyncSpec(fixture),IClassFixture<JwtAuthAndSqlLiteApiFixture>, IClassFixture<SqliteDatabaseFixture>{}
[Collection("Sqlite")]
public class ApiSyncSpecMsClientPrincipalSqlLite(MsClientAuthAndSqlLiteApiFixture fixture):
    ApiSyncSpec(fixture),IClassFixture<MsClientAuthAndSqlLiteApiFixture>, IClassFixture<SqliteDatabaseFixture>{}
[Collection("MsSql")]
public class ApiSyncSpecJwtTokenMsSql(JwtAuthAndMsSqlApiFixture fixture):
    ApiSyncSpec(fixture),IClassFixture<JwtAuthAndMsSqlApiFixture>{}
[Collection("MsSql")]
public class ApiSyncSpecMsClientPrincipalMsSql(MsClientAuthAndMsSqlApiFixture fixture):
    ApiSyncSpec(fixture), IClassFixture<MsClientAuthAndMsSqlApiFixture>{}

public abstract class BaseApiSpec(ApiFixture application)
{
    [Fact]
    public async Task Create_auction_1()
    {
        var response = await application.PostAuction(FirstAuctionRequest, AuthToken.Seller1);
        var id = GetId(JToken.Parse(await response.Content.ReadAsStringAsync()));
        var auction1 = await application.GetAuction(id, AuthToken.Seller1);
        var stringContent = await auction1.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(WithId(JToken.Parse(FirstAuctionResponse), id).ToString(Formatting.Indented),
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }


    [Fact]
    public async Task Create_auction_2()
    {
        var response = await application.PostAuction(SecondAuctionRequest, AuthToken.Seller1);
        var id = GetId(JToken.Parse(await response.Content.ReadAsStringAsync()));
        var auction1 = await application.GetAuction(id, AuthToken.Seller1);
        var stringContent = await auction1.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(WithId(JToken.Parse(SecondAuctionResponse), id).ToString(Formatting.Indented),
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }



    [Fact]
    public async Task Cannot_find_unknown_auction()
    {
        var auctionResponse = await application.GetAuction(99, AuthToken.Seller1);
        Assert.Equal(HttpStatusCode.NotFound, auctionResponse.StatusCode);
    }
    [Theory,
     InlineData("""
                {
                        "startsAt": "2021-12-01T10:00:00.000Z",
                        "endsAt": "2022-12-18T10:00:00.000Z",
                        "title": "Some auction",
                        "currency": "VAC",
                        "timeFrame": -1,
                        "reservePrice": null,
                        "minRaise":null
                    }
                """,1),
     InlineData("""
                {
                        "startsAt": "2021-12-01T10:00:00.000Z",
                        "endsAt": "2022-12-18T10:00:00.000Z",
                        "title": "Some auction",
                    }
                """,2),
    ]
    public async Task Fail_to_create_auction(string sample, int index)
    {
        var response = await application.PostAuction(sample, AuthToken.Seller1);
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        });
    }
    [Fact]
    public async Task Place_bid_as_buyer_on_auction_1()
    {
        var response = await application.PostAuction(FirstAuctionRequest, AuthToken.Seller1);
        var id = GetId(JToken.Parse(await response.Content.ReadAsStringAsync()));
        application.SetTime(StartsAt.AddHours(2));
        var bidResponse = await application.PostBidToAuction(id, """{"amount":"VAC11"}""", AuthToken.Buyer1);
        application.SetTime(EndsAt.AddHours(2));
        var auctionResponse = await application.GetAuction(id, AuthToken.Seller1);
        var bidResponseString = await bidResponse.Content.ReadAsStringAsync();
        var stringContent = await auctionResponse.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(bidResponse.IsSuccessStatusCode);
            Assert.Empty(bidResponseString);
            Assert.Equal(HttpStatusCode.OK, auctionResponse.StatusCode);
            Assert.Equal(WithId(HasEnded(
                    WithPriceAndWinner(
                        WithBid(JToken.Parse(FirstAuctionResponse),"VAC11","#1", "02:00:00"),
                    "VAC11", "#1")),id).ToString(Formatting.Indented),
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }
}
public abstract class ApiSyncSpec(ApiFixture application): BaseApiSpec(application)
{
    [Fact]
    public async Task Cannot_place_bid_on_unknown_auction()
    {
        var bidResponse = await application.PostBidToAuction(999, @"{""amount"":""VAC11""}", AuthToken.Buyer1);
        Assert.Equal(HttpStatusCode.NotFound, bidResponse.StatusCode);
    }
}
