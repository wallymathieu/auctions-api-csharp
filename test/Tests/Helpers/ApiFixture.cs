using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Tests.Helpers;

public class ApiFixture(IDatabaseFixture databaseFixture, IApiAuth auth) : IDisposable, IAsyncLifetime
{
    private readonly FakeSystemClock _fakeSystemClock = new(InitialNow);
    private readonly WebApplicationFactory<Program> _webApplicationFactory = new();

    private TestServer? _testServer;

    public async ValueTask InitializeAsync()
    {
        await databaseFixture.InitializeAsync();
        var application = _webApplicationFactory
            .WithWebHostBuilder(builder =>
            {
                auth.Configure(builder);
                builder.ConfigureServices(services =>
                {
                    databaseFixture.Configurator.Use(services);
                    services.Remove(services.First(s => s.ServiceType == typeof(ISystemClock)));
                    services.AddSingleton<ISystemClock>(_fakeSystemClock);
                });
                builder.UseEnvironment("Test");
            });
        using var serviceScope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        await databaseFixture.Migrator.Migrate(serviceScope);
        _testServer = application.Server;
    }

    public async ValueTask DisposeAsync()
    {
        await databaseFixture.DisposeAsync().ConfigureAwait(false);
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _testServer?.Dispose();
        _webApplicationFactory.Dispose();
    }

    public async Task<HttpResponseMessage> PostAuction(string auctionRequest, AuthToken authToken)
    {
        return await _testServer!.CreateRequest("/auctions").And(r =>
        {
            r.Content = Json(auctionRequest);
            AcceptJson(r);
            auth.TryAddAuth(r, authToken);
        }).PostAsync();
    }

    public async Task<HttpResponseMessage> PostBidToAuction(long id, string bidRequest, AuthToken authToken)
    {
        return await _testServer!.CreateRequest($"/auctions/{id}/bids").And(r =>
        {
            r.Content = Json(bidRequest);
            AcceptJson(r);
            auth.TryAddAuth(r, authToken);
        }).PostAsync();
    }

    private static StringContent Json(string bidRequest)
    {
        return new StringContent(bidRequest, Encoding.UTF8, "application/json");
    }

    public async Task<HttpResponseMessage> GetAuction(long id, AuthToken authToken)
    {
        return await _testServer!.CreateRequest($"/auctions/{id}").And(r =>
        {
            AcceptJson(r);
            auth.TryAddAuth(r, authToken);
        }).GetAsync();
    }

    private static void AcceptJson(HttpRequestMessage r)
    {
        r.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public ITimeSetter CreateSetTimeScope()
    {
        return new FakeClockSetter(_fakeSystemClock);
    }

    /// <summary>
    /// Only intended to reset time back to initial now, so that other tests are not affected by
    /// clock changes.
    /// </summary>
    private sealed class FakeClockSetter(FakeSystemClock fakeSystemClock):ITimeSetter,IDisposable
    {
        public void Dispose()
        {
            fakeSystemClock.Now = InitialNow;
        }

        public void SetTime(DateTimeOffset now)
        {
            fakeSystemClock.Now = now;
        }
    }
}

public interface ITimeSetter:IDisposable
{
    void SetTime(DateTimeOffset now);
}