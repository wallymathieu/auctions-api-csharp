using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Wallymathieu.Auctions.Tests;

public static class JsonHelper
{
    public static int GetId(JToken parsed)
    {
        Debug.Assert(parsed != null, nameof(parsed) + " != null");
        var id = parsed["id"];
        Assert.NotNull(id);
        return id.ToObject<int>();
    }
    public static JToken WithId(JToken parsed, int id)
    {
        Debug.Assert(parsed != null, nameof(parsed) + " != null");
        parsed["id"] = id;
        return parsed;
    }
    public static JToken WithBid(JToken token, string amount, string bidder, string at)
    {
        ArgumentNullException.ThrowIfNull(token, nameof(token));

        JArray array = (JArray)(token["bids"] ?? new JArray());
        array.Add(JToken.Parse($@"{{""amount"": ""{amount}"",""bidder"": ""{bidder}"", ""at"":""{at}""}}"));
        return token;
    }

    public static JToken WithPriceAndWinner(JToken token, string amount, string winner)
    {
        ArgumentNullException.ThrowIfNull(token, nameof(token));

        token["price"] = amount;
        token["winner"] = winner;
        return token;
    }

    public static JToken HasEnded(JToken token)
    {
        ArgumentNullException.ThrowIfNull(token, nameof(token));

        token["hasEnded"] = true;
        return token;
    }
}