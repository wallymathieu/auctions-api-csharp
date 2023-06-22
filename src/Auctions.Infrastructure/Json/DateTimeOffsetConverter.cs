using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wallymathieu.Auctions.Infrastructure.Json;

public class DateTimeOffsetConverter: JsonConverter<DateTimeOffset>
{
    public override bool HandleNull => true;

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTimeOffset.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString(TimeFormatter.TimeFormat));
    }
}