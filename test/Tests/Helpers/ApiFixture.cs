using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Data;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Tests.Helpers;
public enum AuthToken
{
    None,
    Seller1,
    Buyer1,
}
public interface IApiAuth
{
    bool TryAddAuth(HttpRequestMessage r, AuthToken auth);
    void Configure(IWebHostBuilder builder);
}
public class JwtApiAuth: IApiAuth
{
    private static string GetToken(string sub, string email)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes( @"{
""sub"":"""+sub+@""", ""name"":"""+email+@""", ""u_typ"":""0""
}"));
    }
    private readonly string Seller1 = GetToken("a1", "seller1@hotmail.com");
    private readonly string Buyer1 =  GetToken("a2", "buyer1@hotmail.com");
    private static void AddXJwtPayload(HttpRequestMessage r, string auth)
    {
        if (!string.IsNullOrWhiteSpace(auth))
        {
            r.Headers.Add("x-jwt-payload", auth);
        }
    }

    public bool TryAddAuth(HttpRequestMessage r, AuthToken auth)
    {
        switch(auth){
            case AuthToken.Buyer1: AddXJwtPayload(r, Buyer1); return true; 
            case AuthToken.Seller1: AddXJwtPayload(r, Seller1); return true; 
            default: return false;
        }
    }

    public void Configure(IWebHostBuilder builder)
    {
    }
}
public class MsClientPrincipalApiAuth: IApiAuth
{
    private const string XMsClientPrincipal = "X-MS-CLIENT-PRINCIPAL";

    private static string GetToken(string email)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes( @"{
    ""auth_typ"":""aad"",""claims"":[
        {""typ"":""ver"",""val"":""2.0""},
        {""typ"":""iss"",""val"":""https:\/\/login.microsoftonline.com\/sdsdsd\/v2.0""},
        {""typ"":""http:\/\/schemas.xmlsoap.org\/ws\/2005\/05\/identity\/claims\/nameidentifier"",""val"":""00000000-0000-0000-0000-000000000000""},
        {""typ"":""aud"",""val"":""868585685""},
        {""typ"":""exp"",""val"":""1681816329""},
        {""typ"":""iat"",""val"":""1681729629""},
        {""typ"":""nbf"",""val"":""1681729629""},
        {""typ"":""name"",""val"":""Oskar Gewalli""},{""typ"":""preferred_username"",""val"":"""+email+@"""},
        {""typ"":""http:\/\/schemas.microsoft.com\/identity\/claims\/objectidentifier"",""val"":""00000000-0000-0000-0000-000000000000""},
        {""typ"":""http:\/\/schemas.xmlsoap.org\/ws\/2005\/05\/identity\/claims\/emailaddress"",""val"":"""+email+@"""},
        {""typ"":""http:\/\/schemas.microsoft.com\/identity\/claims\/tenantid"",""val"":""00000000-0000-0000-0000-000000000000""},
        {""typ"":""c_hash"",""val"":""-""},
        {""typ"":""nonce"",""val"":""_""},
        {""typ"":""aio"",""val"":""*""}],
    ""name_typ"":""http:\/\/schemas.xmlsoap.org\/ws\/2005\/05\/identity\/claims\/emailaddress"",
    ""role_typ"":""http:\/\/schemas.microsoft.com\/ws\/2008\/06\/identity\/claims\/role""}"));
    }
    private readonly string Seller1 = GetToken("seller1@hotmail.com");
    private readonly string Buyer1 =GetToken("buyer1@hotmail.com");
    private static void AddXJwtPayload(HttpRequestMessage r, string auth)
    {
        if (!string.IsNullOrWhiteSpace(auth))
        {
            r.Headers.Add(XMsClientPrincipal, auth);
        }
    }

    public bool TryAddAuth(HttpRequestMessage r, AuthToken auth)
    {
        switch(auth){
            case AuthToken.Buyer1: AddXJwtPayload(r, Buyer1); return true; 
            case AuthToken.Seller1: AddXJwtPayload(r, Seller1); return true; 
            default: return false;
        }
    }

    public void Configure(IWebHostBuilder builder)
    {
        builder.UseSetting("PrincipalHeader", XMsClientPrincipal);
    }
}

public class ApiFixture<TAuth>:IDisposable where TAuth:IApiAuth, new()
{
    TestServer Create()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                _auth.Configure(builder);
                builder.ConfigureServices(services =>
                {
                    services.Remove(services.First(s => s.ServiceType == typeof(AuctionDbContext)));
                    services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<AuctionDbContext>)));
                    services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions)));
                    services.AddDbContext<AuctionDbContext>(c=>c.UseSqlite("Data Source=" + _db, conf=>conf.MigrationsAssembly("Auctions")));
                   
                    services.Remove(services.First(s => s.ServiceType == typeof(ITime)));
                    services.AddSingleton<ITime>(new FakeTime(new DateTime(2022,8,4)));
                   
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
    private readonly TAuth _auth;

    public ApiFixture(string db)
    {
        _db = db;
        _auth= new TAuth();
        RemoveDbFile();
        _testServer = Create();
    }

    public void Dispose()
    {
        _testServer.Dispose();
        RemoveDbFile();
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
    
}