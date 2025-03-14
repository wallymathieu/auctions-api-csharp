using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace Wallymathieu.Auctions.Tests.Helpers;

public class MsClientPrincipalApiAuth : IApiAuth
{
    private const string XMsClientPrincipal = "X-MS-CLIENT-PRINCIPAL";

    private static string GetToken(string email)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(@"{
    ""auth_typ"":""aad"",""claims"":[
        {""typ"":""ver"",""val"":""2.0""},
        {""typ"":""iss"",""val"":""https:\/\/login.microsoftonline.com\/sdsdsd\/v2.0""},
        {""typ"":""http:\/\/schemas.xmlsoap.org\/ws\/2005\/05\/identity\/claims\/nameidentifier"",""val"":""00000000-0000-0000-0000-000000000000""},
        {""typ"":""aud"",""val"":""868585685""},
        {""typ"":""exp"",""val"":""1681816329""},
        {""typ"":""iat"",""val"":""1681729629""},
        {""typ"":""nbf"",""val"":""1681729629""},
        {""typ"":""name"",""val"":""Oskar Gewalli""},{""typ"":""preferred_username"",""val"":""" + email + @"""},
        {""typ"":""http:\/\/schemas.microsoft.com\/identity\/claims\/objectidentifier"",""val"":""00000000-0000-0000-0000-000000000000""},
        {""typ"":""http:\/\/schemas.xmlsoap.org\/ws\/2005\/05\/identity\/claims\/emailaddress"",""val"":""" + email +
                                                             @"""},
        {""typ"":""http:\/\/schemas.microsoft.com\/identity\/claims\/tenantid"",""val"":""00000000-0000-0000-0000-000000000000""},
        {""typ"":""c_hash"",""val"":""-""},
        {""typ"":""nonce"",""val"":""_""},
        {""typ"":""aio"",""val"":""*""}],
    ""name_typ"":""http:\/\/schemas.xmlsoap.org\/ws\/2005\/05\/identity\/claims\/emailaddress"",
    ""role_typ"":""http:\/\/schemas.microsoft.com\/ws\/2008\/06\/identity\/claims\/role""}"));
    }

    private readonly string Seller1 = GetToken("seller1@hotmail.com");
    private readonly string Buyer1 = GetToken("buyer1@hotmail.com");

    private static void AddXJwtPayload(HttpRequestMessage r, string auth)
    {
        if (!string.IsNullOrWhiteSpace(auth))
        {
            r.Headers.Add(XMsClientPrincipal, auth);
        }
    }

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
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        builder.UseSetting("PrincipalHeader", XMsClientPrincipal);
    }
}