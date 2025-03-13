namespace Wallymathieu.Auctions.DomainModels.Bids;

public class NumberedBidUserMapper : IBidUserMapper
{
    private readonly Dictionary<UserId, string> _userIdToStringMap;

    public NumberedBidUserMapper(ICollection<BidEntity> bids)
    {
        _userIdToStringMap = [];
        var orderedBids = bids.OrderBy(b => b.At).ToArray();
        for (var i = 0; i < orderedBids.Length; i++)
        {
            var bid = orderedBids[i];
            if (!_userIdToStringMap.ContainsKey(bid.User)) _userIdToStringMap.Add(bid.User, $"#{i + 1}");
        }
    }

    public string? GetUserString(UserId? userId)
    {
        return userId != null ? _userIdToStringMap[userId] : null;
    }
}