using System.Net;
using App;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tests.Helpers;

namespace Tests;
public class ApiSpecJwtToken:ApiSpec<JwtApiAuth>{} 
public class ApiSpecMsClientPrincipal:ApiSpec<MsClientPrincipalApiAuth>{} 

public abstract class ApiSpec<TAuth>
    where TAuth:IApiAuth, new()
{
    private const string FirstAuctionRequest = @"{
        ""startsAt"": ""2022-07-01T10:00:00.000Z"",
        ""endsAt"": ""2022-09-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC""
}";

    [Fact]
    public async Task Create_auction_1()
    { 
        using var application = new ApiFixture<TAuth>(typeof(TAuth).Name+"_"+nameof(Create_auction_1)+".db");
        var response = await application.PostAuction(FirstAuctionRequest, AuthToken.Seller1);
        var stringContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(JToken.Parse(@"{
                ""id"": 1,
                ""startsAt"": ""2022-07-01T10:00:00.000Z"",
                ""title"": ""Some auction"",
                ""expiry"": ""2022-09-18T10:00:00.000Z"",
                ""user"": ""seller1@hotmail.com"",
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
        using var application = new ApiFixture<TAuth>(typeof(TAuth).Name+"_"+nameof(Create_auction_2)+".db");
        var response = await application.PostAuction(SecondAuctionRequest, AuthToken.Seller1);
        var stringContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(JToken.Parse(@"{
                ""id"": 1,
                ""startsAt"": ""2021-12-01T10:00:00.000Z"",
                ""title"": ""Some auction"",
                ""expiry"": ""2022-12-18T10:00:00.000Z"",
                ""user"": ""seller1@hotmail.com"",
                ""currency"": ""VAC"",
                ""bids"": []
        }").ToString(Formatting.Indented), 
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
        using var application = new ApiFixture<TAuth>(typeof(TAuth).Name+"_"+nameof(Fail_to_create_auction)+"_"+index+".db");
        var response = await application.PostAuction(sample, AuthToken.Seller1);
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        });
    }
    [Fact]
    public async Task Place_bid_as_buyer_on_auction_1()
    { 
        using var application = new ApiFixture<TAuth>(typeof(TAuth).Name+"_"+nameof(Place_bid_as_buyer_on_auction_1)+".db");
        var response = await application.PostAuction(FirstAuctionRequest, AuthToken.Seller1);
        var bidResponse = await application.PostBidToAuction(1, @"{""amount"":""VAC11""}", AuthToken.Buyer1);
        var auctionResponse = await application.GetAuction(1, AuthToken.Seller1);
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
                ""user"": ""seller1@hotmail.com"",
                ""currency"": ""VAC"",
                ""bids"": [{
                    ""amount"": ""VAC11"",
                    ""bidder"": ""buyer1@hotmail.com""
                }]}").ToString(Formatting.Indented), 
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    } 
}