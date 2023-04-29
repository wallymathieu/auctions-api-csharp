using System.Text.Json;
using System.Text.Json.Serialization;
using Auctions.Domain;

namespace Auctions.Infrastructure.Json;

public class AmountConverter: JsonConverter<Amount>
{
    public override bool HandleNull => true;
    public override Amount Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Amount.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, Amount value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}