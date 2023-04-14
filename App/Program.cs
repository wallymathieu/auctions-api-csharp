using App;
using App.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Startup.Services(builder.Services);
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AuctionDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
Startup.App(app);


app.Run();
