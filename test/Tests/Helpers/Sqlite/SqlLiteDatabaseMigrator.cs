using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Tests.Helpers.Sqlite;

public class SqliteDatabaseMigrator : IDatabaseMigrator
{
    public async ValueTask Migrate(IServiceScope serviceScope)
    {
        ArgumentNullException.ThrowIfNull(serviceScope, nameof(serviceScope));

        var context = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        // NOTE, we would like to use :
        // context.Database.Migrate();
        // however we get the error
        // SQLite Error 1: 'AUTOINCREMENT is only allowed on an INTEGER PRIMARY KEY'.
        await context.Database.EnsureCreatedAsync();
    }
}