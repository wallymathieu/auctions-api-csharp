using Newtonsoft.Json.Linq;
using Wallymathieu.Auctions.Infrastructure;

namespace Wallymathieu.Auctions.Tests;

public static class JsonSamples
{
    public static readonly string FirstAuctionRequest = $@"{{
        ""startsAt"": ""{StartsAt:O}"",
        ""endsAt"": ""{EndsAt:O}"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC""
}}";

    public static readonly string FirstAuctionResponse = $@"{{
                ""id"": 1,
                ""startsAt"":""{TimeFormatter.Format(StartsAt)}"",
                ""title"": ""Some auction"",
                ""expiry"": ""{TimeFormatter.Format(EndsAt)}"",
                ""seller"": ""seller1@hotmail.com"",
                ""currency"": ""VAC"",
                ""bids"": [],
                ""price"": null,
                ""winner"": null,
                ""hasEnded"": false
        }}";
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
                ""bids"": [],
                ""price"": null,
                ""winner"": null,
                ""hasEnded"": false
        }";

    public static JToken WithBid(JToken token, string amount, string bidder, string at)
    {
        JArray array = (JArray)(token["bids"] ?? new JArray());
        array.Add(JToken.Parse($@"{{""amount"": ""{amount}"",""bidder"": ""{bidder}"", ""at"":""{at}""}}"));
        return token;
    }

    public static JToken WithPriceAndWinner(JToken token, string amount, string winner)
    {
        token["price"] = amount;
        token["winner"] = winner;
        return token;
    }

    public static JToken HasEnded(JToken token)
    {
        token["hasEnded"] = true;
        return token;
    }
}