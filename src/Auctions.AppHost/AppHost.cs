var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("redis");
var sql = builder.AddSqlServer("db")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);
var db = sql.AddDatabase("auctions");

var migrationService = builder.AddProject<Projects.Auctions_MigrationService>("migration")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.Auctions_WebApi>("api")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(db)
    .WaitFor(db)
    .WaitFor(migrationService)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Auctions_Frontend>("frontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(db)
    .WaitFor(db)
    .WaitFor(migrationService)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
