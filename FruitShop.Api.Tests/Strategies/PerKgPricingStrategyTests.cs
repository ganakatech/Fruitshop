using FluentAssertions;
using FruitShop.Api.Strategies;

namespace FruitShop.Api.Tests.Strategies;

public class PerKgPricingStrategyTests
{
    [Fact]
    public void CalculatePrice_ShouldMultiplyBasePriceByWeight()
    {
        // Arrange
        var strategy = new PerKgPricingStrategy();
        var basePrice = 2.00m;
        var weight = 3.5m;

        // Act
        var result = strategy.CalculatePrice(basePrice, weight);

        // Assert
        result.Should().Be(7.00m);
    }

    [Fact]
    public void GetUnit_ShouldReturnKg()
    {
        // Arrange
        var strategy = new PerKgPricingStrategy();

        // Act
        var result = strategy.GetUnit();

        // Assert
        result.Should().Be("kg");
    }

    [Fact]
    public void CalculatePrice_WithZeroWeight_ShouldReturnZero()
    {
        // Arrange
        var strategy = new PerKgPricingStrategy();
        var basePrice = 2.00m;
        var weight = 0m;

        // Act
        var result = strategy.CalculatePrice(basePrice, weight);

        // Assert
        result.Should().Be(0m);
    }
}
