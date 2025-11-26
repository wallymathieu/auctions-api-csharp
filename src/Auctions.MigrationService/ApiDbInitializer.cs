using Microsoft.EntityFrameworkCore;
using Wallymathieu.Auctions.Frontend.Data;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Auctions.MigrationService;
using System.Diagnostics;

public class ApiDbInitializer(
    IServiceProvider serviceProvider,
    IHostEnvironment hostEnvironment,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    private readonly ActivitySource _activitySource = new(hostEnvironment.ApplicationName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = _activitySource.StartActivity(hostEnvironment.ApplicationName, ActivityKind.Client);

        await MigrateDbContext<AuctionDbContext>(activity, stoppingToken);

        await MigrateDbContext<FrontendDbContext>(activity, stoppingToken);

        hostApplicationLifetime.StopApplication();
    }

    private async Task MigrateDbContext<TDbContext>(Activity? activity,CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
            });
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }
    }
}