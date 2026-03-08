using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Data;
using FruitShop.Api.Models;
using FruitShop.Api.Services;
using FruitShop.Api.Factories;

namespace FruitShop.Api.Tests.Services;

public class OrderServiceTests
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
    public async Task CalculateTotalAsync_WithSingleItem_ShouldCalculateCorrectly()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new OrderService(context);
        
        var apple = FruitFactory.Create("Apple", 2.00m, "PerKg");
        context.Fruits.Add(apple);
        await context.SaveChangesAsync();

        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { FruitId = apple.Id, Amount = 3.0m } // 3 kg of apples
            }
        };

        // Act
        var result = await service.CalculateTotalAsync(order);

        // Assert
        result.Should().Be(6.00m); // 2.00 * 3.0
        }
    }

    [Fact]
    public async Task CalculateTotalAsync_WithMultipleItems_ShouldCalculateCorrectly()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new OrderService(context);
        
        var apple = FruitFactory.Create("Apple", 2.00m, "PerKg");
        var banana = FruitFactory.Create("Banana", 0.30m, "PerItem");
        context.Fruits.AddRange(apple, banana);
        await context.SaveChangesAsync();

        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { FruitId = apple.Id, Amount = 2.0m }, // 2 kg apples = 4.00
                new OrderItem { FruitId = banana.Id, Amount = 5.0m } // 5 bananas = 1.50
            }
        };

        // Act
        var result = await service.CalculateTotalAsync(order);

        // Assert
        result.Should().Be(5.50m); // 4.00 + 1.50
        }
    }

    [Fact]
    public async Task CalculateTotalAsync_WithDiscountedItem_ShouldApplyDiscount()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new OrderService(context);
        
        var cherry = FruitFactory.Create("Cherry", 5.00m, "Discounted", 2.0m, 10.0m);
        context.Fruits.Add(cherry);
        await context.SaveChangesAsync();

        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { FruitId = cherry.Id, Amount = 3.0m } // 3 kg cherries (exceeds 2.0 threshold)
            }
        };

        // Act
        var result = await service.CalculateTotalAsync(order);

        // Assert
        // Base: 5.00 * 3.0 = 15.00
        // Discount: 15.00 * 0.10 = 1.50
        // Final: 15.00 - 1.50 = 13.50
        result.Should().Be(13.50m);
        }
    }

    [Fact]
    public async Task CalculateTotalAsync_WithMixedPricingStrategies_ShouldCalculateCorrectly()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new OrderService(context);
        
        var apple = FruitFactory.Create("Apple", 2.00m, "PerKg");
        var banana = FruitFactory.Create("Banana", 0.30m, "PerItem");
        var cherry = FruitFactory.Create("Cherry", 5.00m, "Discounted", 2.0m, 10.0m);
        context.Fruits.AddRange(apple, banana, cherry);
        await context.SaveChangesAsync();

        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { FruitId = apple.Id, Amount = 1.5m },  // 1.5 kg = 3.00
                new OrderItem { FruitId = banana.Id, Amount = 10.0m }, // 10 items = 3.00
                new OrderItem { FruitId = cherry.Id, Amount = 3.0m }  // 3 kg with discount = 13.50
            }
        };

        // Act
        var result = await service.CalculateTotalAsync(order);

        // Assert
        result.Should().Be(19.50m); // 3.00 + 3.00 + 13.50
        }
    }

    [Fact]
    public async Task CalculateTotalAsync_WithInvalidFruitId_ShouldThrowArgumentException()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new OrderService(context);

            var order = new Order
            {
                Items = new List<OrderItem>
                {
                    new OrderItem { FruitId = 999, Amount = 1.0m }
                }
            };

            // Act
            var act = async () => await service.CalculateTotalAsync(order);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
