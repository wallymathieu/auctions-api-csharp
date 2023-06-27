namespace Wallymathieu.Auctions.DomainModels;

[Serializable]
public record UserId(string? Id)
{
    public override string? ToString()
    {
        return Id;
    }

    public static UserId NewUserId(string? value)
    {
        return new UserId(value);
    }
}