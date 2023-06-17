namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;
/// <summary>
/// A key value factory that wraps the logic of asking for a value from an instance of a given type <see cref="T"/>.
/// The type in question is most probably a command class.
/// </summary>
/// <typeparam name="T">Type that the instance of the key value factory can return a key for</typeparam>
public interface IKeyValueFactory<in T>
{
    /// <summary>
    /// Get key value of instance of type <see cref="T"/>
    /// </summary>
    /// <param name="obj">an instance of <see cref="T"/></param>
    /// <returns>key value</returns>
    object? Key(T obj);
}