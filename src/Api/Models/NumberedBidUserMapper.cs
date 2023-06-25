using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Api.Models;

public class NumberedBidUserMapper:IBidUserMapper
{
    private readonly IDictionary<UserId,string> _userIdToStringMap;

    public NumberedBidUserMapper(ICollection<BidEntity> bids)
    {
        _userIdToStringMap = new Dictionary<UserId, string>();
        var orderedBids = bids.OrderBy(b => b.At).ToArray();
        for (int i = 0; i < orderedBids.Length; i++)
        {
            var bid = orderedBids[i];
            if (!_userIdToStringMap.ContainsKey(bid.User))
            {
                _userIdToStringMap.Add(bid.User, $"#{i + 1}");
            }
        }
    }

    public string? GetUserString(UserId? userId)
    {
        return userId!=null? _userIdToStringMap[userId] : null;
    }
}