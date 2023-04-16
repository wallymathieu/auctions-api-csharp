using System.Net;
using App;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests;

public class ApiSpec
{
    private const string FirstAuctionRequest = @"{
        ""startsAt"": ""2022-07-01T10:00:00.000Z"",
        ""endsAt"": ""2022-09-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC""
}";

    private const string Seller1 = "eyJzdWIiOiJhMSIsICJuYW1lIjoiVGVzdCIsICJ1X3R5cCI6IjAifQo=";
    private const string Buyer1 = "eyJzdWIiOiJhMiIsICJuYW1lIjoiQnV5ZXIiLCAidV90eXAiOiIwIn0K";
    
    [Fact]
    public async Task Create_auction_1()
    { 
        using var application = new ApiFixture(nameof(Create_auction_1)+".db");
        var response = await application.PostAuction(FirstAuctionRequest, Seller1);
        var stringContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(JToken.Parse(@"{
                ""id"": 1,
                ""startsAt"": ""2022-07-01T10:00:00.000Z"",
                ""title"": ""Some auction"",
                ""expiry"": ""2022-09-18T10:00:00.000Z"",
                ""user"": ""Test"",
                ""currency"": ""VAC"",
                ""bids"": []
        }").ToString(Formatting.Indented), 
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }

    private const string SecondAuctionRequest = @"{
        ""startsAt"": ""2021-12-01T10:00:00.000Z"",
        ""endsAt"": ""2022-12-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC""
}";
    [Fact]
    public async Task Create_auction_2()
    { 
        using var application = new ApiFixture(nameof(Create_auction_2)+".db");
        var response = await application.PostAuction(SecondAuctionRequest, Seller1);
        var stringContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(JToken.Parse(@"{
                ""id"": 1,
                ""startsAt"": ""2021-12-01T10:00:00.000Z"",
                ""title"": ""Some auction"",
                ""expiry"": ""2022-12-18T10:00:00.000Z"",
                ""user"": ""Test"",
                ""currency"": ""VAC"",
                ""bids"": []
        }").ToString(Formatting.Indented), 
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }
    [Fact]
    public async Task Place_bid_as_buyer_on_auction_1()
    { 
        using var application = new ApiFixture(nameof(Place_bid_as_buyer_on_auction_1)+".db");
        var response = await application.PostAuction(FirstAuctionRequest, Seller1);
        var bidResponse = await application.PostBidToAuction(1, @"{""amount"":""VAC11""}", Buyer1);
        var auctionResponse = await application.GetAuction(1, Seller1);
        var bidResponseString = await bidResponse.Content.ReadAsStringAsync();
        var stringContent = await auctionResponse.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(HttpStatusCode.OK,bidResponse.StatusCode);
            Assert.Empty(bidResponseString);
            Assert.Equal(HttpStatusCode.OK, auctionResponse.StatusCode);
            Assert.Equal(JToken.Parse(@"{
                ""id"": 1,
                ""startsAt"": ""2022-07-01T10:00:00.000Z"",
                ""title"": ""Some auction"",
                ""expiry"": ""2022-09-18T10:00:00.000Z"",
                ""user"": ""Test"",
                ""currency"": ""VAC"",
                ""bids"": [{
                    ""amount"": ""VAC11"",
                    ""bidder"": ""Buyer""
                }]}").ToString(Formatting.Indented), 
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    } 
}