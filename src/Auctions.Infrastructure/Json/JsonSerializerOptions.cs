using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wallymathieu.Auctions.Infrastructure.Json;

public static class JsonSerializerOptionsModule
{
    public static JsonSerializerOptions AddAuctionConverters(this JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new DateTimeOffsetConverter());
        options.Converters.Add(new AmountConverter());
        return options;
    }
}