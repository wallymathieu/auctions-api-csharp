namespace Wallymathieu.Auctions.Tests.Helpers;

public interface IApiFixture:IDisposable
{
    Task<HttpResponseMessage> PostAuction(string auctionRequest, AuthToken auth);
    Task<HttpResponseMessage> PostBidToAuction(long id, string bidRequest, AuthToken auth);
    Task<HttpResponseMessage> GetAuction(long id, AuthToken auth);
    void SetTime(DateTimeOffset now);
}
