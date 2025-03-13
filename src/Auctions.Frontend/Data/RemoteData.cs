namespace Wallymathieu.Auctions.Frontend.Data;

public class RemoteData<T>
{
    public T? Data { get; private set; }
    public bool Loaded { get; private set; }

    public void OnLoad(T? data)
    {
        Data = data;
        Loaded = true;
    }
}
