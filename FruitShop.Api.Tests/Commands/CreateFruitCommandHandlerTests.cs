using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MediatR;
using FruitShop.Api.Commands;
using FruitShop.Api.Data;
using FruitShop.Api.Services;
using AutoMapper;
using FruitShop.Api.Mappings;

namespace FruitShop.Api.Tests.Commands;

public class CreateFruitCommandHandlerTests
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
    public async Task Handle_ShouldCreateFruitAndReturnDto()
    {
        // Arrange
        var (context, connection) = CreateDbContext();
        using (context)
        using (connection)
        {
            var service = new FruitService(context);
            var mapper = CreateMapper();
            var handler = new CreateFruitCommandHandler(service, mapper);

            var command = new CreateFruitCommand
            {
                Name = "Apple",
                BasePrice = 2.00m,
                PricingStrategyType = "PerKg"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Apple");
            result.BasePrice.Should().Be(2.00m);
            result.PricingStrategyType.Should().Be("PerKg");
            result.Unit.Should().Be("kg");
            result.Id.Should().BeGreaterThan(0);

            var dbFruit = await context.Fruits.FindAsync(result.Id);
            dbFruit.Should().NotBeNull();
        }
    }
}
