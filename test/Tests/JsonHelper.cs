using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Wallymathieu.Auctions.Tests;

public static class JsonHelper
{
    public static int GetId(JToken parsed)
    {
        ArgumentNullException.ThrowIfNull(parsed);
        var id = parsed["id"];
        Assert.NotNull(id);
        return id.ToObject<int>();
    }

    public static JToken WithId(JToken parsed, int id)
    {
        ArgumentNullException.ThrowIfNull(parsed);
        parsed["id"] = id;
        return parsed;
    }

    public static JToken WithBid(JToken token, long amount, string bidder, string at)
    {
        ArgumentNullException.ThrowIfNull(token);

        JArray array = (JArray)(token["bids"] ?? new JArray());
        array.Add(JToken.Parse($@"{{""amount"": {amount}, ""bidder"": ""{bidder}"", ""at"":""{at}""}}"));
        return token;
    }

    public static JToken WithPriceAndWinner(JToken token, long amount, string winner)
    {
        ArgumentNullException.ThrowIfNull(token);

        token["price"] = amount;
        token["winner"] = winner;
        return token;
    }

    public static JToken HasEnded(JToken token)
    {
        ArgumentNullException.ThrowIfNull(token);

        token["hasEnded"] = true;
        return token;
    }
}