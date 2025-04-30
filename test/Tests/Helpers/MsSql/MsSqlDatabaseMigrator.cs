using FluentMigrator.Runner;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Tests.Helpers.MsSql;

public class MsSqlDatabaseMigrator(string? connection) : IDatabaseMigrator
{
    public async Task Migrate()
    {
        await using var migrations = new Migrations(connection);
        migrations.MigrationRunner().MigrateUp();
    }
}