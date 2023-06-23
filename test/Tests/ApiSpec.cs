using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wallymathieu.Auctions.Tests.Helpers;

namespace Wallymathieu.Auctions.Tests;
using static JsonSamples;
public class ApiSyncSpecJwtToken:ApiSyncSpec<JwtApiAuth>{}
public class ApiSyncSpecMsClientPrincipal:ApiSyncSpec<MsClientPrincipalApiAuth>{}

public abstract class ApiSyncSpec<TAuth>
    where TAuth:IApiAuth, new()
{
    [Fact]
    public async Task Create_auction_1()
    {
        using var application = new ApiFixture<TAuth>(GetType().Name+"_"+nameof(Create_auction_1)+".db");
        var response = await application.PostAuction(FirstAuctionRequest, AuthToken.Seller1);
        var stringContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(JToken.Parse(FirstAuctionResponse).ToString(Formatting.Indented),
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }


    [Fact]
    public async Task Create_auction_2()
    {
        using var application = new ApiFixture<TAuth>(GetType().Name+"_"+nameof(Create_auction_2)+".db");
        var response = await application.PostAuction(SecondAuctionRequest, AuthToken.Seller1);
        var stringContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(JToken.Parse(SecondAuctionResponse).ToString(Formatting.Indented),
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }
    [Theory,
     InlineData(@"{
        ""startsAt"": ""2021-12-01T10:00:00.000Z"",
        ""endsAt"": ""2022-12-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC"",
        ""timeFrame"": -1,
        ""reservePrice"": null,
        ""minRaise"":null
    }",1),
     InlineData(@"{
        ""startsAt"": ""2021-12-01T10:00:00.000Z"",
        ""endsAt"": ""2022-12-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
    }",2),
    ]
    public async Task Fail_to_create_auction(string sample, int index)
    {
        using var application = new ApiFixture<TAuth>(GetType().Name+"_"+nameof(Fail_to_create_auction)+"_"+index+".db");
        var response = await application.PostAuction(sample, AuthToken.Seller1);
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        });
    }
    [Fact]
    public async Task Place_bid_as_buyer_on_auction_1()
    {
        using var application = new ApiFixture<TAuth>(this.GetType().Name+"_"+nameof(Place_bid_as_buyer_on_auction_1)+".db");
        var response = await application.PostAuction(FirstAuctionRequest, AuthToken.Seller1);
        application.SetTime(StartsAt.AddHours(2));
        var bidResponse = await application.PostBidToAuction(1, @"{""amount"":""VAC11""}", AuthToken.Buyer1);
        var bidResponseString = await bidResponse.Content.ReadAsStringAsync();
        application.SetTime(EndsAt.AddHours(2));
        var auctionResponse = await application.GetAuction(1, AuthToken.Seller1);
        var stringContent = await auctionResponse.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(HttpStatusCode.OK,bidResponse.StatusCode);
            Assert.Empty(bidResponseString);
            Assert.Equal(HttpStatusCode.OK, auctionResponse.StatusCode);
            Assert.Equal(WithBid(FirstAuctionResponse,"VAC11","buyer1@hotmail.com").ToString(Formatting.Indented),
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }
    [Fact]
    public async Task Cannot_find_unknown_auction()
    {
        using var application = new ApiFixture<TAuth>(GetType().Name+"_"+nameof(Cannot_find_unknown_auction)+".db");
        var auctionResponse = await application.GetAuction(99, AuthToken.Seller1);
        Assert.Equal(HttpStatusCode.NotFound, auctionResponse.StatusCode);
    }
    [Fact]
    public async Task Cannot_place_bid_on_unknown_auction()
    {
        using var application = new ApiFixture<TAuth>(GetType().Name+"_"+nameof(Cannot_place_bid_on_unknown_auction)+".db");
        var bidResponse = await application.PostBidToAuction(1, @"{""amount"":""VAC11""}", AuthToken.Buyer1);
        Assert.Equal(HttpStatusCode.NotFound, bidResponse.StatusCode);
    }
}
public class ApiAsyncSpecJwtToken:ApiAsyncSpec<JwtApiAuth>{}
public class ApiAsyncSpecMsClientPrincipal:ApiAsyncSpec<MsClientPrincipalApiAuth>{}
public abstract class ApiAsyncSpec<TAuth>
    where TAuth:IApiAuth, new()
{
    [Fact]
    public async Task Create_auction_1()
    {
        using var application = new AsyncApiFixture<TAuth>(GetType().Name+"_"+nameof(Create_auction_1)+".db");
        var response = await application.PostAuction(FirstAuctionRequest, AuthToken.Seller1);
        var auctionResponse = await application.GetAuction(1, AuthToken.Seller1);
        var stringContent = await auctionResponse.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Accepted,response.StatusCode);
            Assert.Equal(HttpStatusCode.OK, auctionResponse.StatusCode);
            Assert.Equal(JToken.Parse(FirstAuctionResponse).ToString(Formatting.Indented),
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }

    [Fact]
    public async Task Create_auction_2()
    {
        using var application = new AsyncApiFixture<TAuth>(GetType().Name+"_"+nameof(Create_auction_2)+".db");
        var response = await application.PostAuction(SecondAuctionRequest, AuthToken.Seller1);
        var auctionResponse = await application.GetAuction(1, AuthToken.Seller1);
        var stringContent = await auctionResponse.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Accepted,response.StatusCode);
            Assert.Equal(HttpStatusCode.OK, auctionResponse.StatusCode);
            Assert.Equal(JToken.Parse(SecondAuctionResponse).ToString(Formatting.Indented),
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }
    [Theory,
     InlineData(@"{
        ""startsAt"": ""2021-12-01T10:00:00.000Z"",
        ""endsAt"": ""2022-12-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC"",
        ""timeFrame"": -1,
        ""reservePrice"": null,
        ""minRaise"":null
    }",1),
     InlineData(@"{
        ""startsAt"": ""2021-12-01T10:00:00.000Z"",
        ""endsAt"": ""2022-12-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
    }",2),
    ]
    public async Task Fail_to_create_auction(string sample, int index)
    {
        using var application = new AsyncApiFixture<TAuth>(GetType().Name+"_"+nameof(Fail_to_create_auction)+"_"+index+".db");
        var response = await application.PostAuction(sample, AuthToken.Seller1);
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        });
    }
    [Fact]
    public async Task Place_bid_as_buyer_on_auction_1()
    {
        using var application = new AsyncApiFixture<TAuth>(GetType().Name+"_"+nameof(Place_bid_as_buyer_on_auction_1)+".db");
        var response = await application.PostAuction(FirstAuctionRequest, AuthToken.Seller1);
        application.SetTime(StartsAt.AddHours(2));
        var bidResponse = await application.PostBidToAuction(1, @"{""amount"":""VAC11""}", AuthToken.Buyer1);
        application.SetTime(EndsAt.AddHours(2));
        var auctionResponse = await application.GetAuction(1, AuthToken.Seller1);
        var bidResponseString = await bidResponse.Content.ReadAsStringAsync();
        var stringContent = await auctionResponse.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Accepted,response.StatusCode);
            Assert.Equal(HttpStatusCode.Accepted,bidResponse.StatusCode);
            Assert.Empty(bidResponseString);
            Assert.Equal(HttpStatusCode.OK, auctionResponse.StatusCode);
            Assert.Equal(WithBid(FirstAuctionResponse,"VAC11","buyer1@hotmail.com").ToString(Formatting.Indented),
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }
    [Fact]
    public async Task Cannot_find_unknown_auction()
    {
        using var application = new AsyncApiFixture<TAuth>(GetType().Name+"_"+nameof(Cannot_find_unknown_auction)+".db");
        var auctionResponse = await application.GetAuction(99, AuthToken.Seller1);
        Assert.Equal(HttpStatusCode.NotFound, auctionResponse.StatusCode);
    }
    [Fact]
    public async Task Place_bid_on_unknown_auction()
    {
        using var application = new AsyncApiFixture<TAuth>(GetType().Name+"_"+nameof(Place_bid_on_unknown_auction)+".db");
        var bidResponse = await application.PostBidToAuction(199, @"{""amount"":""VAC11""}", AuthToken.Buyer1);
        Assert.Equal(HttpStatusCode.Accepted, bidResponse.StatusCode);

    }
}