using FluentAssertions;
using FruitShop.Api.Strategies;

namespace FruitShop.Api.Tests.Strategies;

public class PerItemPricingStrategyTests
{
    [Fact]
    public void CalculatePrice_ShouldMultiplyBasePriceByQuantity()
    {
        // Arrange
        var strategy = new PerItemPricingStrategy();
        var basePrice = 0.30m;
        var quantity = 5m;

        // Act
        var result = strategy.CalculatePrice(basePrice, quantity);

        // Assert
        result.Should().Be(1.50m);
    }

    [Fact]
    public void GetUnit_ShouldReturnItem()
    {
        // Arrange
        var strategy = new PerItemPricingStrategy();

        // Act
        var result = strategy.GetUnit();

        // Assert
        result.Should().Be("item");
    }

    [Fact]
    public void CalculatePrice_WithZeroQuantity_ShouldReturnZero()
    {
        // Arrange
        var strategy = new PerItemPricingStrategy();
        var basePrice = 0.30m;
        var quantity = 0m;

        // Act
        var result = strategy.CalculatePrice(basePrice, quantity);

        // Assert
        result.Should().Be(0m);
    }
}
