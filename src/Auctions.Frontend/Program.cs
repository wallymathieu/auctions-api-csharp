using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Wallymathieu.Auctions.Infrastructure.Web;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.AddAuctionsWebInfrastructure();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOptions<PayloadAuthenticationOptions>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
