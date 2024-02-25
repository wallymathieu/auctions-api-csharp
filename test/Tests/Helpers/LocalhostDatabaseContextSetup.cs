using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Wallymathieu.Auctions.Tests.Helpers;

/// <summary>
/// Database context setup used to configure and setup the database context
/// </summary>
public class LocalhostDatabaseContextSetup : IDatabaseContextSetup
{
    private string? _dbName;

    public void Init(Type testClass, string testName)
    {
        _dbName = $"{testClass.Name}_{testName}";
        ExecuteInConnection($@"CREATE DATABASE ""{_dbName}"";
            GRANT ALL PRIVILEGES ON DATABASE ""{_dbName}"" TO ""auctions-user"" ;");
    }

    public void Use(IServiceCollection services)
    {
    }

    public void Migrate(IServiceScope serviceScope)
    {
    }

    public void TryRemove()
    {
        ExecuteInConnection($@"DROP DATABASE {_dbName};");
    }

    public void Configure(IWebHostBuilder builder) => builder.UseConfiguration(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            {"ConnectionStrings:DefaultConnection", DefaultConnection(_dbName!)},
            {"ConnectionStrings:Redis", ""},
            {"ConnectionStrings:AzureStorage",""}
        }).Build());

    private static string DefaultConnection(string database = "auctions")
    {
        var password = Environment.GetEnvironmentVariable("SA_PASSWORD");
        var username = Environment.GetEnvironmentVariable("SA_USERNAME") ?? "auctions-user";
        return $"Host=localhost;Database={database};Port=5432;Username={username};Password={password}";
    }

    private void ExecuteInConnection(string commandText) =>
        ExecuteInConnectionAsync(commandText).ConfigureAwait(false).GetAwaiter().GetResult();
    private async Task ExecuteInConnectionAsync(string commandText)
    {
        var dataSourceBuilder = new NpgsqlConnectionStringBuilder(DefaultConnection());
        var dataSource = dataSourceBuilder.ConnectionString;

        using (var conn = new NpgsqlConnection(dataSource))
        {
            await conn.OpenAsync();
            var command = conn.CreateCommand();
            command.CommandText = commandText;
            await command.ExecuteNonQueryAsync();
        }
    }
}