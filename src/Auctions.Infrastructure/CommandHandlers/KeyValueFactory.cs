using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;

public class KeyValueFactory<T>: IKeyValueFactory<T>
{
    readonly PropertyInfo _property;
    public KeyValueFactory()
    {
        var type = typeof(T);
        var properties = type.GetProperties();
        var property = properties.FirstOrDefault(p =>
            p.CustomAttributes?.Any(attr => attr.AttributeType == typeof(KeyAttribute))??false) ?? properties.FirstOrDefault(p =>
            p.Name!=null
            && p.Name.Equals("ID", StringComparison.OrdinalIgnoreCase));
        if (property is null && typeof(IEntity).IsAssignableFrom(type))
        {
            property = properties.FirstOrDefault(p =>
                p.Name!=null
                && p.Name.Equals(type.Name + "ID", StringComparison.OrdinalIgnoreCase));
        }

        _property = property ?? throw new Exception("No key found for type") { Data = { { "Type", typeof(T) } } };
    }
    public object? Key(T obj) => _property.GetValue(obj);
}