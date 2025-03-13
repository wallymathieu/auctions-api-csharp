namespace Wallymathieu.Auctions.Tests.Helpers.Sqlite;

/// <summary>
/// Database context setup used to configure and setup the database context
/// </summary>
public class SqliteDatabaseFixture : IDatabaseFixture
{
    private string? _db;
    private string? _connectionString;

    public Task InitializeAsync()
    {
        _db = $"{Guid.NewGuid():N}.db";
        _connectionString = $"Data Source={_db}";
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        if (File.Exists(_db))
        {
            try
            {
                File.Delete(_db);
            }
            catch
            {
                // ignored
            }
        }

        return Task.CompletedTask;
    }

    public IDatabaseConfigurator Configurator
    {
        get
        {
            if (_connectionString is null)
                throw new InvalidOperationException(
                    $"Connection string not initialized {nameof(_connectionString)}"
                );
            return new SqliteDatabaseConfigurator(_connectionString);
        }
    }

    public IDatabaseMigrator Migrator => new SqliteDatabaseMigrator();
}
