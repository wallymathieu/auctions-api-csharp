using System.Net.Http.Headers;
using System.Text;

namespace Wallymathieu.Auctions.Tests.Helpers.Http;

public class ApiClientBase(
    string singleAuction = "/auction",
    string auctions = "/auctions") : IAuctionClient
{
    public async Task<HttpResponseMessage> PostAuction(HttpClient client, IApiAuth auth,
        string auctionRequest, AuthToken authToken)
    {
        using var r = new HttpRequestMessage(HttpMethod.Post, singleAuction);
        r.Content = Json(auctionRequest);
        AcceptJson(r);
        auth.TryAddAuth(r, authToken);
        return await client.SendAsync(r);
    }

    public async Task<HttpResponseMessage> PostBidToAuction(HttpClient client, IApiAuth auth,
        long id, string bidRequest, AuthToken authToken)
    {
        using var r = new HttpRequestMessage(HttpMethod.Post, $"{singleAuction}/{id}/bids");
        r.Content = Json(bidRequest);
        AcceptJson(r);
        auth.TryAddAuth(r, authToken);
        return await client.SendAsync(r);
    }

    private static StringContent Json(string bidRequest)
    {
        return new StringContent(bidRequest, Encoding.UTF8, "application/json");
    }

    public async Task<HttpResponseMessage> GetAuction(HttpClient client, IApiAuth auth,
        long id, AuthToken authToken)
    {
        using var r = new HttpRequestMessage(HttpMethod.Get, $"{singleAuction}/{id}");
        AcceptJson(r);
        auth.TryAddAuth(r, authToken);
        return await client.SendAsync(r);
    }

    public async Task<HttpResponseMessage> GetAuctions(HttpClient client, IApiAuth auth,
        AuthToken authToken)
    {
        using var r = new HttpRequestMessage(HttpMethod.Get, $"{auctions}");
        AcceptJson(r);
        auth.TryAddAuth(r, authToken);
        return await client.SendAsync(r);
    }

    private static void AcceptJson(HttpRequestMessage r)
    {
        r.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}