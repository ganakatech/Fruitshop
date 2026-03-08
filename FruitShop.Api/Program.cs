using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using FluentValidation;
using MediatR;
using FruitShop.Api.Data;
using FruitShop.Api.Services;
using FruitShop.Api.Behaviors;
using FruitShop.Api.Models;
using FruitShop.Api.Factories;
using FruitShop.Api.Middleware;
using FruitShop.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Create and keep a SQLite in-memory connection open
// SQLite in-memory databases are connection-specific and require the connection to stay open
var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

// Configure SQLite in-memory database using the open connection
builder.Services.AddDbContext<FruitShopDbContext>(options =>
    options.UseSqlite(connection));

// Configure MediatR with pipeline behaviors
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// Configure FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Enable annotations for better Swagger documentation
    c.EnableAnnotations();
});

// Configure Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<FruitShopDbContext>();

// Register services
builder.Services.AddScoped<IFruitService, FruitService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Ensure database is created and seeded on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FruitShopDbContext>();
    dbContext.Database.EnsureCreated();
    SeedData(dbContext);
}

// Configure Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure health check endpoint
app.MapHealthChecks("/health");

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

// Add global exception handling middleware (should be early in the pipeline)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Map API endpoints
app.MapApiEndpoints();

// Ensure connection is disposed when app shuts down
app.Lifetime.ApplicationStopped.Register(() => connection.Dispose());

app.Run();

// Seed data method
static void SeedData(FruitShopDbContext dbContext)
{
    if (dbContext.Fruits.Any())
    {
        return; // Already seeded
    }

    var fruits = new[]
    {
        FruitFactory.Create("Apple", 2.00m, "PerKg"),
        FruitFactory.Create("Banana", 0.30m, "PerItem"),
        FruitFactory.Create("Cherry", 5.00m, "Discounted", 2.0m, 10.0m)
    };

    dbContext.Fruits.AddRange(fruits);
    dbContext.SaveChanges();
}
