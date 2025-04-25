namespace Wallymathieu.Auctions.Tests.Helpers.Http;

public interface IAuctionClient
{
    Task<HttpResponseMessage> PostAuction(HttpClient client, IApiAuth auth, string auctionRequest, AuthToken authToken);
    Task<HttpResponseMessage> PostBidToAuction(HttpClient client, IApiAuth auth, long id, string bidRequest, AuthToken authToken);
    Task<HttpResponseMessage> GetAuction(HttpClient client, IApiAuth auth, long id, AuthToken authToken);
    Task<HttpResponseMessage> GetAuctions(HttpClient client, IApiAuth auth, AuthToken authToken);
}