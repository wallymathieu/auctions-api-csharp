using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wallymathieu.Auctions.Functions;
using Wallymathieu.Auctions.Infrastructure.Cache;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Json;
using Wallymathieu.Auctions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((builder,services) =>
    {
        services
            .AddAuctionQueryCached()
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString(ConnectionStrings.Redis);
                options.InstanceName = CacheKeys.Prefix;
            })
            .AddAuctionMartenStore(builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection)!)
            .AddAuctionServicesCached()
            .AddScoped<ScopedUserContext>()
            .AddScoped<IUserContext>(c=>c.GetRequiredService<ScopedUserContext>());
        services.AddSingleton(new JsonSerializerOptions().AddAuctionConverters());
    })
    .Build();

host.Run();
