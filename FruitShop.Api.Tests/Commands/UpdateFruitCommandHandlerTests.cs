using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Commands;
using FruitShop.Api.Data;
using FruitShop.Api.Services;
using FruitShop.Api.Factories;
using AutoMapper;
using FruitShop.Api.Mappings;

namespace FruitShop.Api.Tests.Commands;

public class UpdateFruitCommandHandlerTests
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

    private IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<FruitMappingProfile>());
        return new Mapper(configuration);
    }

    [Fact]
    public async Task Handle_ShouldUpdateFruitAndReturnDto()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
            var mapper = CreateMapper();
            var handler = new UpdateFruitCommandHandler(service, mapper);

            var fruit = FruitFactory.Create("Apple", 2.00m, "PerKg");
            context.Fruits.Add(fruit);
            await context.SaveChangesAsync();

            var command = new UpdateFruitCommand
            {
                Id = fruit.Id,
                Name = "Updated Apple",
                BasePrice = 2.50m,
                PricingStrategyType = "PerKg"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Updated Apple");
            result.BasePrice.Should().Be(2.50m);
        }
    }

    [Fact]
    public async Task Handle_WithInvalidId_ShouldThrowArgumentException()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
            var mapper = CreateMapper();
            var handler = new UpdateFruitCommandHandler(service, mapper);

            var command = new UpdateFruitCommand
            {
                Id = 999,
                Name = "Test",
                BasePrice = 1.00m,
                PricingStrategyType = "PerKg"
            };

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
