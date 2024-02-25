using Marten;
using Marten.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wallymathieu.Auctions.Infrastructure.Services;

namespace Wallymathieu.Auctions.Infrastructure.Data;

public static class DataExtensions
{
    internal static IServiceCollection AddAuctionQueryImplementation(this IServiceCollection services)
    {
        services.TryAddScoped<AuctionQuery>();
        return services;
    }

    public static async Task<Auction?> GetAuction(this IDocumentSession session, AuctionId auctionId, CancellationToken token= default)
    {
        return await session.LoadAsync<Auction>(
            id: auctionId.Id,
            token: token);
    }
    public static async Task<IReadOnlyList<Auction>> GetAuctionsAsync(this IDocumentSession session, CancellationToken token= default)
    {
        return await session.Query<Auction>().ToListAsync<Auction>(token);
    }

    public static IServiceCollection AddAuctionQueryNoCache(this IServiceCollection services)
    {
        return AddAuctionQueryImplementation(services)
            .AddScoped<IAuctionQuery>(c=>
                c.GetRequiredService<AuctionQuery>());
    }
    public static IServiceCollection AddAuctionMartenStore(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddMarten(options =>
        {
            options.Schema.For<Auction>().Identity(x => x.Id)
                .AddSubClass<SingleSealedBidAuction>()
                .AddSubClass<TimedAscendingAuction>();
            options.Connection(connectionString);
        }).UseDirtyTrackedSessions();
        return serviceCollection;
    }
}