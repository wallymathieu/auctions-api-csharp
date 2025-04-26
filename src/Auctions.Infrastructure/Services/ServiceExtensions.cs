using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wallymathieu.Auctions.Infrastructure.CommandHandlers;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Services;

public static class ServiceExtensions
{
    internal static IServiceCollection AddAuctionServicesImplementation(this IServiceCollection services)
    {
        services.TryAddSingleton<ISystemClock, SystemClock>();
        services.RegisterAttributesForType<Auction>();
        services.AddSingleton(typeof(IKeyValueFactory<>), typeof(KeyValueFactory<>));
        services.AddScoped<IRequestHandler<CreateAuctionCommand, Auction>>(c =>
            c.GetRequiredService<ICommandHandler<CreateAuctionCommand, Auction>>());
        services.AddScoped<IRequestHandler<CreateBidCommand, Result<Bid, Errors>>>(c =>
            c.GetRequiredService<ICommandHandler<CreateBidCommand, Result<Bid, Errors>>>());
        return services;
    }

    public static IServiceCollection AddAuctionServicesNoCache(this IServiceCollection services)
    {
        AddAuctionServicesImplementation(services);
        services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssemblyContaining<CreateAuctionQueuePipeLineBehavior>();
            c.AddBehavior<CreateAuctionQueuePipeLineBehavior>();
            c.AddBehavior<CreateBidQueuePipeLineBehavior>();
        });
        return services;
    }
}