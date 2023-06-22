using Newtonsoft.Json.Linq;

namespace Wallymathieu.Auctions.Tests;

public static class JsonSamples
{
    public const string FirstAuctionRequest = @"{
        ""startsAt"": ""2022-07-01T10:00:00.000Z"",
        ""endsAt"": ""2022-09-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC""
}";

    public const string FirstAuctionResponse = @"{
                ""id"": 1,
                ""startsAt"": ""2022-07-01T10:00:00.000Z"",
                ""title"": ""Some auction"",
                ""expiry"": ""2022-09-18T10:00:00.000Z"",
                ""seller"": ""seller1@hotmail.com"",
                ""currency"": ""VAC"",
                ""bids"": []
        }";
    public const string SecondAuctionRequest = @"{
        ""startsAt"": ""2021-12-01T10:00:00.000Z"",
        ""endsAt"": ""2022-12-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC""
}";

    public const string SecondAuctionResponse = @"{
                ""id"": 1,
                ""startsAt"": ""2021-12-01T10:00:00.000Z"",
                ""title"": ""Some auction"",
                ""expiry"": ""2022-12-18T10:00:00.000Z"",
                ""seller"": ""seller1@hotmail.com"",
                ""currency"": ""VAC"",
                ""bids"": []
        }";

    public static JToken WithBid(string auction, string amount, string bidder)
    {
        var token = JToken.Parse(auction);
        JArray array = (JArray)(token["bids"] = token["bids"] ?? new JArray());
        array.Add(JToken.Parse($@"{{""amount"": ""{amount}"",""bidder"": ""{bidder}""}}"));
        return token;
    }
}