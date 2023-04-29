namespace Wallymathieu.Auctions.Domain;

[Serializable]
public record UserId(string Id)
{
    public override string ToString()
    {
        return Id;
    }

    public static UserId NewUserId(string value)
    {
        return new UserId(value);
    }
}