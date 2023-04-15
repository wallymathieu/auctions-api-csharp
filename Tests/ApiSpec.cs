using System.Net;
using System.Security.Claims;
using System.Text;
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
        static TestServer Create()
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
                        services.AddControllers(c => c.Filters.Add<ApiKeyAuthorizationFilter>());
                    });
                });
            using var serviceScope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();
            context.Database.EnsureCreated();
            return application.Server;
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
        
    }

    private const string firstAuctionRequest = @"{
            ""id"": 1,
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
        using var application = new ApiFixture();
        var response = await application.Server.CreateRequest("/auctions").And(r =>
        {
            r.Content = new StringContent(firstAuctionRequest, Encoding.UTF8, "application/json");
            r.Headers.Add("x-jwt-payload", Seller1);
        }).PostAsync();
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
                ""currency"": ""VAC""
        }").ToString(Formatting.Indented), 
                JToken.Parse(stringContent).ToString(Formatting.Indented));
        });
    }
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
