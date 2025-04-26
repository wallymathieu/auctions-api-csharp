namespace Wallymathieu.Auctions.Tests.Helpers.Sqlite;

/// <summary>
///     Database context setup used to configure and setup the database context
/// </summary>
public sealed class SqliteDatabaseFixture : IDatabaseFixture
{
    private string? _connectionString;
    private string? _db;

    public ValueTask InitializeAsync()
    {
        _db = $"{Guid.NewGuid():N}.db";
        _connectionString = $"Data Source={_db}";
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        if (File.Exists(_db))
            try
            {
                File.Delete(_db);
            }
#pragma warning disable CA1031 // these files can be locked, but we do not really care if they are unable to be deleted
            catch
#pragma warning restore CA1031
            {
                // ignored
            }

        return ValueTask.CompletedTask;
    }

    public IDatabaseConfigurator Configurator
    {
        get
        {
            if (_connectionString is null)
                throw new InvalidOperationException(
                    $"Connection string not initialized {nameof(_connectionString)}");
            return new SqliteDatabaseConfigurator(_connectionString);
        }
    }

    public IDatabaseMigrator Migrator => new SqliteDatabaseMigrator();
}