using System.Net.Http.Headers;
using System.Text;
using App.Data;
using Auctions.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests;

public class ApiFixture:IDisposable
{
    TestServer Create()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Remove(services.First(s => s.ServiceType == typeof(AuctionDbContext)));
                    services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<AuctionDbContext>)));
                    services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions)));
                    services.AddDbContext<AuctionDbContext>(c=>c.UseSqlite("Data Source=" + _db));
                    services.AddSingleton<ApiJwtDecodedAuthorizationFilter>();
                    services.AddSingleton<IApiJwtDecodedValidator, ApiJwtDecodedValidator>();
                    services.Remove(services.First(s => s.ServiceType == typeof(ITime)));
                    services.AddSingleton<ITime>(new FakeTime(new DateTime(2022,8,4)));
                    services.AddControllers(c => c.Filters.Add<ApiJwtDecodedAuthorizationFilter>());
                });
            });
        using var serviceScope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        context.Database.EnsureCreated();
        return application.Server;
    }

    private void RemoveDbFile()
    {
        if (File.Exists(_db))
        {
            try
            {
                File.Delete(_db);
            }
            catch
            {
                // ignored
            }
        }
    }

    private readonly TestServer _testServer;
    private readonly string _db;

    public ApiFixture(string db)
    {
        _db = db;
        RemoveDbFile();
        _testServer = Create();
    }

    public void Dispose()
    {
        _testServer.Dispose();
        RemoveDbFile();
    }
    public TestServer Server=>_testServer;

    public async Task<HttpResponseMessage> PostAuction(string auctionRequest, string auth) =>
        await Server.CreateRequest("/auctions").And(r =>
        {
            r.Content = Json(auctionRequest);
            AcceptJson(r);
            AddXJwtPayload(r, auth);
        }).PostAsync();

    public async Task<HttpResponseMessage> PostBidToAuction(long id, string bidRequest, string auth) =>
        await Server.CreateRequest($"/auctions/{id}/bids").And(r =>
        {
            r.Content = Json(bidRequest);
            AcceptJson(r);
            AddXJwtPayload(r, auth);
        }).PostAsync();
    private static StringContent Json(string bidRequest) => new(bidRequest, Encoding.UTF8, "application/json");
    public async Task<HttpResponseMessage> GetAuction(long id, string auth)=>
        await Server.CreateRequest($"/auctions/{id}").And(r =>
        {
            AcceptJson(r);
            AddXJwtPayload(r, auth);
        }).GetAsync();
    private static void AcceptJson(HttpRequestMessage r) => r.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    private static void AddXJwtPayload(HttpRequestMessage r, string auth)
    {
        if (!string.IsNullOrWhiteSpace(auth))
        {
            r.Headers.Add("x-jwt-payload", auth);
        }
    }
}