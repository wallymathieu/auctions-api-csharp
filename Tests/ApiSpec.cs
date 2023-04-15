using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using App;
using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tests;

public class ApiSpec
{
    public class ApiFixture:IDisposable
    {
   
        TestServer Create()
        {
            RemoveDbFile(db);

            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.Remove(services.First(s => s.ServiceType == typeof(AuctionDbContext)));
                        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<AuctionDbContext>)));
                        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions)));
                        services.AddDbContext<AuctionDbContext>(c=>c.UseSqlite("Data Source=" + db));
                        services.AddSingleton<ApiKeyAuthorizationFilter>();
                        services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
                        services.Remove(services.First(s => s.ServiceType == typeof(ITime)));
                        services.AddSingleton<ITime>(new FakeTime(new DateTime(2022,8,4)));
                        services.AddControllers(c => c.Filters.Add<ApiKeyAuthorizationFilter>());
                    });
                });
            using var serviceScope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();
            context.Database.EnsureCreated();
            return application.Server;
        }

        private void RemoveDbFile(string db)
        {
            if (File.Exists(db))
            {
                try
                {
                    File.Delete(db);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private readonly TestServer _testServer;
        private readonly string db;

        public ApiFixture(string db)
        {
            this.db = db;
            _testServer = Create();
        }

        public void Dispose()
        {
            _testServer.Dispose();
            RemoveDbFile(db);
        }
        public TestServer Server=>_testServer;

    }
    
    private static async Task<HttpResponseMessage> PostAction(ApiFixture application, string auctionRequest, string auth) =>
        await application.Server.CreateRequest("/auctions").And(r =>
        {
            r.Content = Json(auctionRequest);
            AcceptJson(r);
            AddXJwtPayload(r, auth);
        }).PostAsync();

    private static async Task<HttpResponseMessage> PostToAction(ApiFixture application, long id, string bidRequest, string auth) =>
        await application.Server.CreateRequest($"/auctions/{id}/bids").And(r =>
        {
            r.Content = Json(bidRequest);
            AcceptJson(r);
            AddXJwtPayload(r, auth);
        }).PostAsync();

    private static StringContent Json(string bidRequest) => new(bidRequest, Encoding.UTF8, "application/json");

    private static async Task<HttpResponseMessage> GetAuction(ApiFixture application, long id, string auth)=>
        await application.Server.CreateRequest($"/auctions/{id}").And(r =>
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

    private const string firstAuctionRequest = @"{
        ""startsAt"": ""2022-07-01T10:00:00.000Z"",
        ""endsAt"": ""2022-09-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC""
}";

    private const string Seller1 = "eyJzdWIiOiJhMSIsICJuYW1lIjoiVGVzdCIsICJ1X3R5cCI6IjAifQo=";
    private const string Buyer1 = "eyJzdWIiOiJhMiIsICJuYW1lIjoiQnV5ZXIiLCAidV90eXAiOiIwIn0K";
    
    [Fact]
    public async Task Create_auction_1()
    { 
        using var application = new ApiFixture(nameof(Create_auction_1)+".db");
        var response = await PostAction(application, firstAuctionRequest, Seller1);
        var stringContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(JToken.Parse(@"{
                ""id"": 1,
                ""startsAt"": ""2022-07-01T10:00:00.000Z"",
                ""title"": ""Some auction"",
                ""expiry"": ""2022-09-18T10:00:00.000Z"",
                ""user"": ""Test"",
                ""currency"": ""VAC"",
                ""bids"": []
        }").ToString(Formatting.Indented), 
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }

    private const string secondAuctionRequest = @"{
        ""startsAt"": ""2021-12-01T10:00:00.000Z"",
        ""endsAt"": ""2022-12-18T10:00:00.000Z"",
        ""title"": ""Some auction"",
        ""currency"": ""VAC""
}";
    [Fact]
    public async Task Create_auction_2()
    { 
        using var application = new ApiFixture(nameof(Create_auction_2)+".db");
        var response = await PostAction(application, secondAuctionRequest, Seller1);
        var stringContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(JToken.Parse(@"{
                ""id"": 1,
                ""startsAt"": ""2021-12-01T10:00:00.000Z"",
                ""title"": ""Some auction"",
                ""expiry"": ""2022-12-18T10:00:00.000Z"",
                ""user"": ""Test"",
                ""currency"": ""VAC"",
                ""bids"": []
        }").ToString(Formatting.Indented), 
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }
    [Fact]
    public async Task Place_bid_as_buyer_on_auction_1()
    { 
        using var application = new ApiFixture(nameof(Place_bid_as_buyer_on_auction_1)+".db");
        var response = await PostAction(application, firstAuctionRequest, Seller1);
        var bidResponse = await PostToAction(application, 1, @"{""amount"":""VAC11""}", Buyer1);
        var auctionResponse = await GetAuction(application, 1, Seller1);
        var bidResponseString = await bidResponse.Content.ReadAsStringAsync();
        var stringContent = await auctionResponse.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(HttpStatusCode.OK,bidResponse.StatusCode);
            Assert.Empty(bidResponseString);
            Assert.Equal(HttpStatusCode.OK, auctionResponse.StatusCode);
            Assert.Equal(JToken.Parse(@"{
                ""id"": 1,
                ""startsAt"": ""2022-07-01T10:00:00.000Z"",
                ""title"": ""Some auction"",
                ""expiry"": ""2022-09-18T10:00:00.000Z"",
                ""user"": ""Test"",
                ""currency"": ""VAC"",
                ""bids"": [{
                    ""amount"": ""VAC11"",
                    ""bidder"": ""Buyer""
                }]}").ToString(Formatting.Indented), 
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    } 
}

internal class FakeTime : ITime
{
    private readonly DateTimeOffset _now;

    public FakeTime(DateTimeOffset now)
    {
        _now = now;
    }

    public DateTimeOffset Now => this._now;
}

internal class JwtPayload
{
    [JsonProperty("sub")]
    public string Sub { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("u_typ")]
    public string UTyp { get; set; }
}

public class ApiKeyAuthorizationFilter:IAuthorizationFilter
{
    private const string ApiKeyHeaderName = "x-jwt-payload";

    private readonly IApiKeyValidator _apiKeyValidator;

    public ApiKeyAuthorizationFilter(IApiKeyValidator apiKeyValidator)
    {
        _apiKeyValidator = apiKeyValidator;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];

        if (!_apiKeyValidator.IsValid(apiKey, out var identify))
        {
            context.Result = new UnauthorizedResult();
        }
        else
        {
            context.HttpContext.User = identify;
        }
    }
}

public interface IApiKeyValidator
{
    bool IsValid(string? apiKey, out ClaimsPrincipal? claimsIdentity);
}

public class ApiKeyValidator:IApiKeyValidator
{
    private ILogger<ApiKeyValidator> _logger;

    public ApiKeyValidator(ILogger<ApiKeyValidator> logger)
    {
        _logger = logger;
    }

    public bool IsValid(string? apiKey, out ClaimsPrincipal? claimsIdentity)
    {
        claimsIdentity = null;
        if (string.IsNullOrWhiteSpace(apiKey)) return false;
        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(apiKey));
            var deserialized = JsonConvert.DeserializeObject<JwtPayload>(json);
            if (deserialized == null) return false;
            claimsIdentity = new ClaimsPrincipal(new []
            {
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new(ClaimTypes.Name,deserialized.Name),
                    })
            });
            return true;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to decode JWT body header"); 
            return false;
        }
    }
}
