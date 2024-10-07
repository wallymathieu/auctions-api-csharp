using System.Text.RegularExpressions;

namespace Wallymathieu.Auctions.DomainModels;

[Serializable]
public abstract partial record User(UserId Id)
{
    [Serializable]
    private record BuyerOrSeller(UserId Id, string? Name) : User(Id)
    {
        public override string ToString()
        {
            return $"BuyerOrSeller|{Id}|{Name}";
        }
    }


    [Serializable]
    private record Support(UserId Id) : User(Id)
    {
        public override string ToString() => $"Support|{Id}";
    }


    public static User NewBuyerOrSeller(UserId id, string? name)
    {
        return new BuyerOrSeller(id, name);
    }

    public static User NewSupport(UserId id)
    {
        return new Support(id);
    }

    public static bool TryParse(string user, out User? value)
    {
        Match match = UserRegex().Match(user);
        if (match.Success)
        {
            var typ = match.Groups["type"].Value;
            var id = match.Groups["id"].Value;
            var name = match.Groups["name"].Value;
            switch (typ)
            {
                case "BuyerOrSeller":
                    value = NewBuyerOrSeller(UserId.NewUserId(id), name);
                    return true;
                case "Support":
                    value = NewSupport(UserId.NewUserId(id));
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        value = default;
        return false;
    }

    [GeneratedRegex("(?<type>\\w*)\\|(?<id>[^|]*)(\\|(?<name>.*))?")]
    private static partial Regex UserRegex();
}