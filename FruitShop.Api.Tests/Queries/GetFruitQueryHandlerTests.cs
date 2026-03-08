using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Queries;
using FruitShop.Api.Data;
using FruitShop.Api.Services;
using FruitShop.Api.Factories;
using AutoMapper;
using FruitShop.Api.Mappings;

namespace FruitShop.Api.Tests.Queries;

public class GetFruitQueryHandlerTests
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
    public async Task Handle_WithValidId_ShouldReturnFruitDto()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
            var mapper = CreateMapper();
            var handler = new GetFruitQueryHandler(service, mapper);

            var fruit = FruitFactory.Create("Apple", 2.00m, "PerKg");
            context.Fruits.Add(fruit);
            await context.SaveChangesAsync();

            var query = new GetFruitQuery { Id = fruit.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Apple");
            result.BasePrice.Should().Be(2.00m);
            result.Unit.Should().Be("kg");
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
            var mapper = CreateMapper();
            var handler = new GetFruitQueryHandler(service, mapper);

            var query = new GetFruitQuery { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Fruit with ID 999 not found.");
        }
    }
}
