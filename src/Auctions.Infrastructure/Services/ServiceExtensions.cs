using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wallymathieu.Auctions.Infrastructure.Cache;
using Wallymathieu.Auctions.Infrastructure.Cache.Services;
using Wallymathieu.Auctions.Services;
namespace Wallymathieu.Auctions.Infrastructure.Services;
public static class ServiceExtensions
{
    internal static IServiceCollection AddAuctionServicesImplementation(this IServiceCollection services)
    {
        services.TryAddSingleton<ITime, Time>();
        services.TryAddScoped<CreateAuctionCommandHandler>();
        services.TryAddScoped<CreateBidCommandHandler>();
        services.TryAddScoped<InnerService<ICreateAuctionCommandHandler>>(c=>
            new InnerService<ICreateAuctionCommandHandler>(c.GetRequiredService<CreateAuctionCommandHandler>()));
        services.TryAddScoped<InnerService<ICreateBidCommandHandler>>(c=>
            new InnerService<ICreateBidCommandHandler>(c.GetRequiredService<CreateBidCommandHandler>()));
        return services;
    }

    public static IServiceCollection AddAuctionServicesNoCache(this IServiceCollection services) =>
        AddAuctionServicesImplementation(services)
            .AddScoped<ICreateAuctionCommandHandler>(c=>
                    c.GetRequiredService<InnerService<ICreateAuctionCommandHandler>>().Service)
            .AddScoped<ICreateBidCommandHandler>(c=>
                    c.GetRequiredService<InnerService<ICreateBidCommandHandler>>().Service);

}