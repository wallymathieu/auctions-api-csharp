using App.Data;
using Auctions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx,services) =>
    {
        services.AddAuctionServices()
            .AddAuctionDbContextSqlServer(ctx.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection))
            .AddAuctionRedisCache(ctx.Configuration.GetConnectionString(ConnectionStrings.Redis));
    })
    .Build();

host.Run();
