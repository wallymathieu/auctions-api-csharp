using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wallymathieu.Auctions.Infrastructure.Json;

public class AmountConverter: JsonConverter<Amount>
{
    public override bool HandleNull => true;
    public override Amount? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType== JsonTokenType.Null
            ? null
            : Amount.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, Amount? value, JsonSerializerOptions options)
    {
        if (value is not null)
            writer.WriteStringValue(value.ToString());
        else
            writer.WriteNullValue();
    }
}