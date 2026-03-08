using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Commands;
using FruitShop.Api.Data;
using FruitShop.Api.Services;
using FruitShop.Api.Factories;

namespace FruitShop.Api.Tests.Commands;

public class DeleteFruitCommandHandlerTests
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
    public async Task Handle_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
            var handler = new DeleteFruitCommandHandler(service);

            var fruit = FruitFactory.Create("Apple", 2.00m, "PerKg");
            context.Fruits.Add(fruit);
            await context.SaveChangesAsync();

            var command = new DeleteFruitCommand { Id = fruit.Id };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var dbFruit = await context.Fruits.FindAsync(fruit.Id);
            dbFruit.Should().BeNull();
        }
    }

    [Fact]
    public async Task Handle_WithInvalidId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
            var handler = new DeleteFruitCommandHandler(service);

            var command = new DeleteFruitCommand { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Fruit with ID 999 not found.");
        }
    }
}
