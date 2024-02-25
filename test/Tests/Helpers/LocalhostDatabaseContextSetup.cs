using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wallymathieu.Auctions.Tests.Helpers;

/// <summary>
/// Database context setup used to configure and setup the database context
/// </summary>
public class LocalhostDatabaseContextSetup : IDatabaseContextSetup
{
    public void Init(Type testClass, string testName)
    {
    }

    public void Use(IServiceCollection services)
    {
    }

    public void Migrate(IServiceScope serviceScope)
    {
    }

    public void TryRemove()
    {
    }

    public void Configure(IWebHostBuilder builder)
    {
        builder.UseConfiguration(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            {"ConnectionStrings:DefaultConnection", DefaultConnection()},
            {"ConnectionStrings:Redis", ""},
            {"ConnectionStrings:AzureStorage",""}
        }).Build());
    }

    private static string DefaultConnection()
    {
        var password = Environment.GetEnvironmentVariable("SA_PASSWORD");
        return "Host=localhost;Database=auctions;Port=5432;Username=auctions-user;Password=" + password;
    }
}