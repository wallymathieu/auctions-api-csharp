using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wallymathieu.Auctions.Tests.Helpers;
using Wallymathieu.Auctions.Tests.Helpers.MsSql;
using Wallymathieu.Auctions.Tests.Helpers.Sqlite;

namespace Wallymathieu.Auctions.Tests;

using static JsonSamplesV1;
using static JsonHelper;
#pragma warning disable CA2000 // these objects are disposed by the class consuming them
public class JwtAuthAndSqlLiteApiFixture() :
    ApiFixture(new SqliteDatabaseFixture(), new JwtApiAuth());

public class MsClientAuthAndSqlLiteApiFixture() :
    ApiFixture(new SqliteDatabaseFixture(), new MsClientPrincipalApiAuth());

public class JwtAuthAndMsSqlApiFixture() :
    ApiFixture(new MsSqlDatabaseFixture(), new JwtApiAuth());

public class MsClientAuthAndMsSqlApiFixture() :
    ApiFixture(new MsSqlDatabaseFixture(), new MsClientPrincipalApiAuth());
#pragma warning restore CA2000

[Collection("Sqlite")]
public class ApiSyncSpecJwtTokenSqlLite(JwtAuthAndSqlLiteApiFixture fixture) :
    ApiSyncSpec(fixture), IClassFixture<JwtAuthAndSqlLiteApiFixture>, IClassFixture<SqliteDatabaseFixture>
{
}

[Collection("Sqlite")]
public class ApiSyncSpecMsClientPrincipalSqlLite(MsClientAuthAndSqlLiteApiFixture fixture) :
    ApiSyncSpec(fixture), IClassFixture<MsClientAuthAndSqlLiteApiFixture>, IClassFixture<SqliteDatabaseFixture>
{
}

[Collection("MsSql")]
public class ApiSyncSpecJwtTokenMsSql(JwtAuthAndMsSqlApiFixture fixture) :
    ApiSyncSpec(fixture), IClassFixture<JwtAuthAndMsSqlApiFixture>
{
}

[Collection("MsSql")]
public class ApiSyncSpecMsClientPrincipalMsSql(MsClientAuthAndMsSqlApiFixture fixture) :
    ApiSyncSpec(fixture), IClassFixture<MsClientAuthAndMsSqlApiFixture>
{
}

public abstract class BaseApiSpec(ApiFixture application)
{
    public const string AuctionUrl = "/auctions";

    [Fact]
    public async Task Create_auction_1()
    {
        var response = await application.Post(AuctionUrl, FirstAuctionRequest, AuthToken.Seller1);
        if (!response.IsSuccessStatusCode)
            Assert.Fail($"Failed to create auction: {response.StatusCode}");

        var id = GetId(JToken.Parse(await response.Content.ReadAsStringAsync()));
        var auction1 = await application.Get($"{AuctionUrl}/{(long)id}", AuthToken.Seller1);
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
        var response = await application.Post(AuctionUrl, SecondAuctionRequest, AuthToken.Seller1);
        if (!response.IsSuccessStatusCode)
            Assert.Fail($"Failed to create auction: {response.StatusCode}");

        var id = GetId(JToken.Parse(await response.Content.ReadAsStringAsync()));
        var auction1 = await application.Get($"{AuctionUrl}/{(long)id}", AuthToken.Seller1);
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
        var auctionResponse = await application.Get($"{AuctionUrl}/{(long)99}", AuthToken.Seller1);
        Assert.Equal(HttpStatusCode.NotFound, auctionResponse.StatusCode);
    }

    [Theory]
    [InlineData("""
                {
                        "startsAt": "2021-12-01T10:00:00.000Z",
                        "endsAt": "2022-12-18T10:00:00.000Z",
                        "title": "Some auction",
                        "currency": "VAC",
                        "type": "English|-1"
                }
                """, 1)]
    [InlineData("""
                {
                        "startsAt": "2021-12-01T10:00:00.000Z",
                        "endsAt": "2022-12-18T10:00:00.000Z",
                        "title": "Some auction",
                        "type": "English|1|2|Bla"
                }
                """, 2)]
    public async Task Fail_to_create_auction(string sample, int index)
    {
        var response = await application.Post(AuctionUrl, sample, AuthToken.Seller1);
        Assert.Multiple(() => { Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); });
    }

    [Fact]
    public async Task Place_bid_as_buyer_on_auction_1()
    {
        var response = await application.Post(AuctionUrl, FirstAuctionRequest, AuthToken.Seller1);
        var id = GetId(JToken.Parse(await response.Content.ReadAsStringAsync()));
        using (var setTimeScope = application.CreateSetTimeScope())
        {
            setTimeScope.SetTime(StartsAt.AddHours(2));
            var bidResponse = await application.Post($"{AuctionUrl}/{id}/bids", """{"amount":11}""", AuthToken.Buyer1);
            setTimeScope.SetTime(EndsAt.AddHours(2));
            var auctionResponse = await application.Get($"{AuctionUrl}/{(long)id}", AuthToken.Seller1);
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
                            WithBid(JToken.Parse(FirstAuctionResponse), 11, "#1", "02:00:00"),
                            11, "#1")), id).ToString(Formatting.Indented),
                    JToken.Parse(stringContent).ToString(Formatting.Indented));
            });
        }
    }
}

public abstract class ApiSyncSpec(ApiFixture application) : BaseApiSpec(application)
{
    [Fact]
    public async Task Cannot_place_bid_on_unknown_auction()
    {
        var bidResponse = await application.Post($"{AuctionUrl}/{999}/bids", @"{""amount"":11}", AuthToken.Buyer1);
        Assert.Equal(HttpStatusCode.NotFound, bidResponse.StatusCode);
    }
}