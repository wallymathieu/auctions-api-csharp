using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wallymathieu.Auctions.Infrastructure.CommandHandlers;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

public static class ServiceExtensions
{
    internal static IServiceCollection AddAuctionServicesImplementation(this IServiceCollection services)
    {
        services.TryAddSingleton<ISystemClock, SystemClock>();
        services.RegisterAttributesForType<Auction>();
        services.AddSingleton(typeof(IKeyValueFactory<>), typeof(KeyValueFactory<>));
        return services;
    }

    public static IServiceCollection AddAuctionServicesNoCache(this IServiceCollection services) =>
        AddAuctionServicesImplementation(services)
            .AddScoped<ICreateAuctionCommandHandler>(c=>
                    new CreateAuctionQueueDecoratedCommandHandler(
                        c.GetRequiredService<InnerService<ICreateAuctionCommandHandler>>().Service,
                        c.GetRequiredService<IMessageQueue>(),
                        c.GetRequiredService<IUserContext>()))
            .AddScoped<ICreateBidCommandHandler>(c=>
                    new CreateBidQueueDecoratedCommandHandler(
                        c.GetRequiredService<InnerService<ICreateBidCommandHandler>>().Service,
                        c.GetRequiredService<IMessageQueue>(),
                        c.GetRequiredService<IUserContext>()))
            .AddScoped<ICreateAuctionCommandHandler>(c =>
                c.GetRequiredService<InnerService<ICreateAuctionCommandHandler>>().Service)
            .AddScoped<ICreateBidCommandHandler>(c =>
                c.GetRequiredService<InnerService<ICreateBidCommandHandler>>().Service);
}