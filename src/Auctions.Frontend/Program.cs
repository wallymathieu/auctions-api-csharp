using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wallymathieu.Auctions.Frontend.Data;
using Wallymathieu.Auctions.Infrastructure.Services;
using Wallymathieu.Auctions.Infrastructure.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<FrontendDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection),
        opt => opt.MigrationsHistoryTable("__FrontendMigrations")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<FrontendDbContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.AddAuctionsWebInfrastructure();
builder.Services
    .AddHttpContextAccessor()
    .AddHttpContextUserContext();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();