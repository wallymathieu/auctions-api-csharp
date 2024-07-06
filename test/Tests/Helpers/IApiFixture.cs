namespace Wallymathieu.Auctions.Tests.Helpers;

public interface IApiFixture: IAsyncLifetime
{
    Task<HttpResponseMessage> PostAuction(string auctionRequest, AuthToken authToken);
    Task<HttpResponseMessage> PostBidToAuction(long id, string bidRequest, AuthToken authToken);
    Task<HttpResponseMessage> GetAuction(long id, AuthToken authToken);
    void SetTime(DateTimeOffset now);
}
