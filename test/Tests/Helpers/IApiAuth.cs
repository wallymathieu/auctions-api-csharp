using Microsoft.AspNetCore.Hosting;

namespace Wallymathieu.Auctions.Tests.Helpers;

public interface IApiAuth
{
    bool TryAddAuth(HttpRequestMessage r, AuthToken auth);
    void Configure(IWebHostBuilder builder);
}
