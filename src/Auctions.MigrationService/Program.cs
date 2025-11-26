using Auctions.MigrationService;
using Microsoft.EntityFrameworkCore;
using Wallymathieu.Auctions.Frontend.Data;
using Wallymathieu.Auctions.Infrastructure.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ApiDbInitializer>();

builder.AddServiceDefaults();

builder.Services.AddDbContextPool<AuctionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("auctions"),
        opt => opt.MigrationsAssembly(MigrationAssembly.Name)));
builder.EnrichSqlServerDbContext<AuctionDbContext>();

builder.Services.AddDbContextPool<FrontendDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("auctions"),
        opt => opt.MigrationsAssembly(typeof(FrontendDbContext).Assembly)));
builder.EnrichSqlServerDbContext<FrontendDbContext>();

var app = builder.Build();

app.Run();