using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Migrations;

namespace Wallymathieu.Auctions.Infrastructure.Data;

public sealed class Migrations : IDisposable, IAsyncDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public Migrations(string? connection)
    {
        _serviceProvider = new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb =>
            {
                rb
                    .AddSqlServer()
                    // Set the connection string
                    .WithGlobalConnectionString(connection)
                    // Define the assembly containing the migrations, maintenance migrations and other customizations
                    .ScanIn(typeof(AddAuctionTable).Assembly).For.All();
            })
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(validateScopes: false);
    }

    public IMigrationRunner MigrationRunner() => _serviceProvider.GetRequiredService<IMigrationRunner>();

    void IDisposable.Dispose() => _serviceProvider.Dispose();

    public async ValueTask DisposeAsync() => await _serviceProvider.DisposeAsync();
}