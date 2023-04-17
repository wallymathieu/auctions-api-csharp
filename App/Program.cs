using System.Text.Json.Serialization;
using App;
using App.Data;
using App.Middleware.Auth;
using Auctions.Domain;
using Auctions.Json;
using Auctions.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers(c => c.Filters.Add<DecodedHeaderAuthorizationFilter>()).AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opts.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
    opts.JsonSerializerOptions.Converters.Add(new AmountConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.MapType(typeof(Amount), () => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString(Amount.Zero(CurrencyCode.VAC).ToString())
    });
});
builder.Services.AddSingleton<ITime, Time>();
builder.Services.AddDbContext<AuctionDbContext>(e=>e.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<DecodedHeaderAuthorizationFilter>();
if (!string.IsNullOrEmpty(builder.Configuration["PrincipalHeader"]))
{
    builder.Services.AddSingleton<IClaimsPrincipalParser, ClaimsPrincipalParser>();
}
else
{
    builder.Services.AddSingleton<IClaimsPrincipalParser, JwtPayloadClaimsPrincipalParser>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
