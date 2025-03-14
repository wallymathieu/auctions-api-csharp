using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Wallymathieu.Auctions.DomainModels;
using Xunit.Sdk;

namespace Wallymathieu.Auctions.Tests.Helpers;

public class AuctionXunitSerializer:IXunitSerializer
{
    public object Deserialize(Type type, string serializedValue)
    {
        return JsonSerializer.Deserialize(serializedValue, type)!;
    }

    public bool IsSerializable(Type type, object? value, [NotNullWhen(false)] out string? failureReason)
    {
        failureReason = null;
        if (value is Auction _) return true;
        ArgumentNullException.ThrowIfNull(type);
        if (type.IsAssignableFrom(typeof(Auction))) return true;
        failureReason = "Value is not a Auction";
        return false;
    }

    public string Serialize(object value)
    {
        return JsonSerializer.Serialize(value);
    }
}