using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Tests.Helpers.MsSql;

public class MsSqlDatabaseMigrator : IDatabaseMigrator
{
    public async Task Migrate(IServiceScope serviceScope)
    {
        ArgumentNullException.ThrowIfNull(serviceScope);
        var context = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        await context.Database.MigrateAsync();
    }
}