namespace Auctions.Services;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuctionServices(this IServiceCollection services)
    {
        services.AddSingleton<ITime, Time>()
            .AddSingleton<Mapper>()
            .AddScoped<CreateAuctionCommandHandler>()
            .AddScoped<CreateBidCommandHandler>();
        return services;
    }

    public static IServiceCollection AddAuctionRedisCache(this IServiceCollection services, string? connectionString)
    {
        return services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "auctions";
        });
    }
}

public static class CacheExtensions
{
}