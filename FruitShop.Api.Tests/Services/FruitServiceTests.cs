using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Data;
using FruitShop.Api.Models;
using FruitShop.Api.Services;
using FruitShop.Api.Factories;

namespace FruitShop.Api.Tests.Services;

public class FruitServiceTests
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
    public async Task GetAllAsync_ShouldReturnAllFruits()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
        
        var fruit1 = FruitFactory.Create("Apple", 2.00m, "PerKg");
        var fruit2 = FruitFactory.Create("Banana", 0.30m, "PerItem");
        context.Fruits.AddRange(fruit1, fruit2);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.Name == "Apple");
        result.Should().Contain(f => f.Name == "Banana");
        result.All(f => f.PricingStrategy != null).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnFruit()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
        
        var fruit = FruitFactory.Create("Apple", 2.00m, "PerKg");
        context.Fruits.Add(fruit);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetByIdAsync(fruit.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Apple");
        result.PricingStrategy.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnFruit()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
            var fruit = FruitFactory.Create("Apple", 2.00m, "PerKg");

            // Act
            var result = await service.CreateAsync(fruit);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Apple");
            result.PricingStrategy.Should().NotBeNull();
            
            var dbFruit = await context.Fruits.FindAsync(result.Id);
            dbFruit.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAndReturnFruit()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
            
            var fruit = FruitFactory.Create("Apple", 2.00m, "PerKg");
            context.Fruits.Add(fruit);
            await context.SaveChangesAsync();

            fruit.Name = "Updated Apple";
            fruit.BasePrice = 2.50m;

            // Act
            var result = await service.UpdateAsync(fruit);

            // Assert
            result.Name.Should().Be("Updated Apple");
            result.BasePrice.Should().Be(2.50m);
            
            var dbFruit = await context.Fruits.FindAsync(fruit.Id);
            dbFruit!.Name.Should().Be("Updated Apple");
        }
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
            
            var fruit = FruitFactory.Create("Apple", 2.00m, "PerKg");
            context.Fruits.Add(fruit);
            await context.SaveChangesAsync();

            // Act
            var result = await service.DeleteAsync(fruit.Id);

            // Assert
            result.Should().BeTrue();
            var dbFruit = await context.Fruits.FindAsync(fruit.Id);
            dbFruit.Should().BeNull();
        }
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);

            // Act
            var result = await service.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }
    }
}
