using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wallymathieu.Auctions.Infrastructure.Json;

public class DateTimeOffsetConverter: JsonConverter<DateTimeOffset>
{
    public override bool HandleNull => true;

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value)
            ? DateTimeOffset.MinValue
            : DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        writer.WriteStringValue(TimeFormatter.Format(value.ToUniversalTime()));
    }
}