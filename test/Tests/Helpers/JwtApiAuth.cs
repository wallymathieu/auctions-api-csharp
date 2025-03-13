using System.Text;
using Microsoft.AspNetCore.Hosting;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

namespace Wallymathieu.Auctions.Tests.Helpers;

public class JwtApiAuth : IApiAuth
{
    private readonly string Buyer1 = GetToken("a2", "buyer1@hotmail.com");
    private readonly string Seller1 = GetToken("a1", "seller1@hotmail.com");

    public bool TryAddAuth(HttpRequestMessage r, AuthToken auth)
    {
        ArgumentNullException.ThrowIfNull(r);
        switch (auth)
        {
            case AuthToken.Buyer1:
                AddXJwtPayload(r, Buyer1);
                return true;
            case AuthToken.Seller1:
                AddXJwtPayload(r, Seller1);
                return true;
            default: return false;
        }
    }

    public void Configure(IWebHostBuilder builder)
    {
    }

    private static string GetToken(string sub, string email)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(@"{
""sub"":""" + sub + @""", ""name"":""" + email + @""", ""u_typ"":""0""
}"));
    }

    private static void AddXJwtPayload(HttpRequestMessage r, string auth)
    {
        if (!string.IsNullOrWhiteSpace(auth)) r.Headers.Add(JwtPayloadClaimsPrincipal.Header, auth);
    }
}