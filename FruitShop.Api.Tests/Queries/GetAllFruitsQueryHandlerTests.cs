using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Queries;
using FruitShop.Api.Data;
using FruitShop.Api.Services;
using FruitShop.Api.Factories;
using AutoMapper;
using FruitShop.Api.Mappings;

namespace FruitShop.Api.Tests.Queries;

public class GetAllFruitsQueryHandlerTests
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
    public async Task Handle_ShouldReturnAllFruitsAsDtos()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
            var mapper = CreateMapper();
            var handler = new GetAllFruitsQueryHandler(service, mapper);

            var fruit1 = FruitFactory.Create("Apple", 2.00m, "PerKg");
            var fruit2 = FruitFactory.Create("Banana", 0.30m, "PerItem");
            context.Fruits.AddRange(fruit1, fruit2);
            await context.SaveChangesAsync();

            var query = new GetAllFruitsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(f => f.Name == "Apple");
            result.Should().Contain(f => f.Name == "Banana");
        }
    }
}
