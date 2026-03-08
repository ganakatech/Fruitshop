using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Queries;
using FruitShop.Api.Data;
using FruitShop.Api.Models;
using FruitShop.Api.Services;
using FruitShop.Api.Factories;

namespace FruitShop.Api.Tests.Queries;

public class CalculateOrderTotalQueryHandlerTests
{
    private (FruitShopDbContext, Microsoft.Data.Sqlite.SqliteConnection) CreateDbContext()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<FruitShopDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new FruitShopDbContext(options);
        context.Database.EnsureCreated();
        return (context, connection);
    }

    [Fact]
    public async Task Handle_ShouldCalculateOrderTotal()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new OrderService(context);
            var handler = new CalculateOrderTotalQueryHandler(service);

            var apple = FruitFactory.Create("Apple", 2.00m, "PerKg");
            var banana = FruitFactory.Create("Banana", 0.30m, "PerItem");
            context.Fruits.AddRange(apple, banana);
            await context.SaveChangesAsync();

            var query = new CalculateOrderTotalQuery
            {
                Order = new Order
                {
                    Items = new List<OrderItem>
                    {
                        new OrderItem { FruitId = apple.Id, Amount = 2.0m },
                        new OrderItem { FruitId = banana.Id, Amount = 5.0m }
                    }
                }
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(5.50m); // 4.00 + 1.50
        }
    }
}
