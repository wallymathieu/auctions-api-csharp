using App;
using App.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests;

public class ApiSpec
{
    public class ApiFixture:IDisposable
    {
        static TestServer Create()
        {
            return new TestServer(new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(new ConfigurationBuilder().Build())
                .UseStartup<TestStartup>()) { AllowSynchronousIO=true };
        }
        private readonly TestServer _testServer;
        public ApiFixture() => _testServer = Create();
        public void Dispose()
        {
            _testServer.Dispose();
            if (!File.Exists(db)) return;
            try{ File.Delete(db); }
            catch
            {
                // ignored
            }
        }
        public TestServer Server=>_testServer;

        const string db = "ApiFixture.db";
        class TestStartup 
        {
            public void ConfigureServices(IServiceCollection services)
            {
                Startup.Services(services);
                services.AddDbContext<AuctionDbContext>(c=>c.UseSqlite("Data Source=" + db));
            }

            public void Configure(WebApplication app)
            {
                Startup.App(app);
                using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var context = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();
                context.Database.Migrate();
            }
        }
    }

    private string firstAuctionRequest = @"{
            ""id"": 1,
        ""startsAt"": ""2022-07-01T10:00:00.000Z"",
        ""endsAt"": ""2022-09-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC""
}";

    private string seller1 = "eyJzdWIiOiJhMSIsICJuYW1lIjoiVGVzdCIsICJ1X3R5cCI6IjAifQo=";
    private string buyer1 = "eyJzdWIiOiJhMiIsICJuYW1lIjoiQnV5ZXIiLCAidV90eXAiOiIwIn0K";
    
    [Fact]
    public async Task Create_auction_1()
    { 
        using var application = new ApiFixture();
        var data = new StringContent(firstAuctionRequest);
        var response = await application.Server.CreateRequest("/auctions").And(r =>
        {
            r.Content = data;
            r.Headers.Add("x-jwt-payload", seller1);
        }).PostAsync();
        Assert.Equal(JToken.Parse(@"{
        ""$type"": ""AuctionAdded"",
        ""at"": ""2022-08-04T00:00:00.000Z"",
        ""auction"": {
            ""id"": 1,
            ""startsAt"": ""2022-07-01T10:00:00.000Z"",
            ""title"": ""Some auction"",
            ""expiry"": ""2022-09-18T10:00:00.000Z"",
            ""user"": ""BuyerOrSeller|a1|Test"",
            ""type"": ""English|VAC0|VAC0|0"",
            ""currency"": ""VAC""
        }
    }").ToString(Formatting.Indented), 
            JToken.Parse( await response.Content.ReadAsStringAsync()).ToString(Formatting.Indented));
    }
}