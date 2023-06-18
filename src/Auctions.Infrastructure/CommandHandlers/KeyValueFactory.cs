using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;

public class KeyValueFactory<T>: IKeyValueFactory<T>
{
    readonly PropertyInfo _property;
    public KeyValueFactory()
    {
        var type = typeof(T);
        var properties = type.GetProperties().ToList();
        var property = properties.Find(HasKeyAttribute)
                       ?? properties.Find(p => NamedId(p));
        if (property is null && typeof(IEntity).IsAssignableFrom(type))
        {
            property = properties.Find(p => NamedId(p, prefix: type.Name));
        }

        _property = property ?? throw new Exception("No key found for type") { Data = { { "Type", typeof(T) } } };
    }

    private static bool HasKeyAttribute(PropertyInfo p) =>
        p.CustomAttributes?.Any(attr => attr.AttributeType == typeof(KeyAttribute)) ?? false;

    private static bool NamedId(PropertyInfo p, string? prefix = null) =>
        p.Name.Equals((prefix ?? "")+"ID", StringComparison.OrdinalIgnoreCase);

    public object? Key(T obj) => _property.GetValue(obj);
}