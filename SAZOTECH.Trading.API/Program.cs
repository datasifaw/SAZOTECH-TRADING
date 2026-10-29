
using Microsoft.EntityFrameworkCore;
using SAZOTECH.Trading.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Register API services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure SQL Server database connection
var connectionString = builder.Configuration
    .GetConnectionString("TradingDb")
    ?? throw new InvalidOperationException(
        "TradingDb connection string is not configured.");

builder.Services.AddDbContext<TradingDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Enable API documentation in development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "SAZOTECH Trading API v1"
        );
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
