using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Services;
using Wallymathieu.Auctions.Tests.Helpers.Http;

namespace Wallymathieu.Auctions.Tests.Helpers;

public class ApiFixture(IDatabaseFixture databaseFixture, IApiAuth auth, IAuctionClient client)
    : IDisposable, IAsyncLifetime
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

    public async Task<HttpResponseMessage> PostAuction(string auctionRequest, AuthToken authToken) =>
        await client.PostAuction(_testServer!.CreateClient(), auth, auctionRequest, authToken);

    public async Task<HttpResponseMessage> PostBidToAuction(long id, string bidRequest, AuthToken authToken) =>
        await client.PostBidToAuction(_testServer!.CreateClient(), auth, id, bidRequest, authToken);

    public async Task<HttpResponseMessage> GetAuction(long id, AuthToken authToken)
    {
        return await client.GetAuction(_testServer!.CreateClient(), auth, id, authToken);
    }

    public ITimeSetter CreateSetTimeScope()
    {
        return new FakeClockSetter(_fakeSystemClock);
    }

    /// <summary>
    /// Only intended to reset time back to initial now, so that other tests are not affected by
    /// clock changes.
    /// </summary>
    private sealed class FakeClockSetter(FakeSystemClock fakeSystemClock) : ITimeSetter
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

public interface ITimeSetter : IDisposable
{
    void SetTime(DateTimeOffset now);
}