
var builder = WebApplication.CreateBuilder(args);

// Register API services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

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
