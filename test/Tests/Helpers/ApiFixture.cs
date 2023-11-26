using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Tests.Helpers;
public class ApiFixture<TAuth>:IApiFixture where TAuth:IApiAuth
{
    private readonly FakeSystemClock _fakeSystemClock= new(InitialNow);
    private readonly IDatabaseContextSetup _databaseContextSetup;
    TestServer Create()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                _auth.Configure(builder);
                builder.ConfigureServices(services =>
                {
                    _databaseContextSetup.Use(services);
                    services.Remove(services.First(s => s.ServiceType == typeof(ISystemClock)));
                    services.AddSingleton<ISystemClock>(_fakeSystemClock);
                    ConfigureServices(services);
                });
                builder.UseEnvironment("Test");
            });
        using var serviceScope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _databaseContextSetup.Migrate(serviceScope);
        return application.Server;
    }


    protected virtual void ConfigureServices(IServiceCollection services) {}



    private readonly TestServer _testServer;
    private readonly TAuth _auth;

    public ApiFixture(IDatabaseContextSetup databaseContextSetup, TAuth auth)
    {
        _databaseContextSetup = databaseContextSetup;
        _auth = auth;
        _testServer = Create();
    }

    public void Dispose()
    {
        _testServer.Dispose();
        _databaseContextSetup.TryRemove().ConfigureAwait(false).GetAwaiter().GetResult();
    }
    public TestServer Server=>_testServer;

    public async Task<HttpResponseMessage> PostAuction(string auctionRequest, AuthToken auth) =>
        await Server.CreateRequest("/auctions").And(r =>
        {
            r.Content = Json(auctionRequest);
            AcceptJson(r);
            _auth.TryAddAuth(r, auth);
        }).PostAsync();

    public async Task<HttpResponseMessage> PostBidToAuction(long id, string bidRequest, AuthToken auth) =>
        await Server.CreateRequest($"/auctions/{id}/bids").And(r =>
        {
            r.Content = Json(bidRequest);
            AcceptJson(r);
            _auth.TryAddAuth(r, auth);
        }).PostAsync();
    private static StringContent Json(string bidRequest) => new(bidRequest, Encoding.UTF8, "application/json");
    public async Task<HttpResponseMessage> GetAuction(long id, AuthToken auth)=>
        await Server.CreateRequest($"/auctions/{id}").And(r =>
        {
            AcceptJson(r);
            _auth.TryAddAuth(r, auth);
        }).GetAsync();
    private static void AcceptJson(HttpRequestMessage r) => r.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    public void SetTime(DateTimeOffset now) => _fakeSystemClock.Now = now;
}