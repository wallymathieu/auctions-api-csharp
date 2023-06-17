using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Infrastructure.CommandHandlers;
using Wallymathieu.Auctions.Infrastructure.Services.Cache;
using Wallymathieu.Auctions.Services;
namespace Wallymathieu.Auctions.Infrastructure.Services;

public static class ServiceExtensions
{
    private static IServiceCollection AddAuctionServicesImplementation(this IServiceCollection services)
    {
        services.TryAddSingleton<ITime, Time>();
        services.TryAddSingleton<Mapper>();
        services.RegisterAttributesForType<Auction>();
        services.AddSingleton(typeof(IKeyValueFactory<>), typeof(KeyValueFactory<>));
        return services;
    }

    public static IServiceCollection AddAuctionServicesNoCache(this IServiceCollection services) =>
        AddAuctionServicesImplementation(services);

    public static IServiceCollection AddAuctionServicesCached(this IServiceCollection services) =>
        AddAuctionServicesImplementation(services)
            .AddScoped<ICommandHandler<CreateAuctionCommand, Auction>>(c=>
                new CacheAwareCreateAuctionCommandHandler(
                    c.GetRequiredService<ICommandHandler<CreateAuctionCommand, Auction>>(),
                    c.GetRequiredService<IDistributedCache>()))
            .AddScoped<ICommandHandler<CreateBidCommand, IResult<Bid,Errors>>>(c=>
                new CacheAwareCreateBidCommandHandler(
                    c.GetRequiredService<ICommandHandler<CreateBidCommand, IResult<Bid,Errors>>>(),
                    c.GetRequiredService<IDistributedCache>()));
}