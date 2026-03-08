using FluentAssertions;
using FruitShop.Api.Strategies;

namespace FruitShop.Api.Tests.Strategies;

public class DiscountedPricingStrategyTests
{
    [Fact]
    public void CalculatePrice_WhenAmountExceedsThreshold_ShouldApplyDiscount()
    {
        // Arrange
        var baseStrategy = new PerKgPricingStrategy();
        var strategy = new DiscountedPricingStrategy(baseStrategy, 2.0m, 10.0m);
        var basePrice = 5.00m;
        var amount = 3.0m; // Exceeds threshold of 2.0

        // Act
        var result = strategy.CalculatePrice(basePrice, amount);

        // Assert
        // Base price: 5.00 * 3.0 = 15.00
        // Discount: 15.00 * 0.10 = 1.50
        // Final: 15.00 - 1.50 = 13.50
        result.Should().Be(13.50m);
    }

    [Fact]
    public void CalculatePrice_WhenAmountDoesNotExceedThreshold_ShouldNotApplyDiscount()
    {
        // Arrange
        var baseStrategy = new PerKgPricingStrategy();
        var strategy = new DiscountedPricingStrategy(baseStrategy, 2.0m, 10.0m);
        var basePrice = 5.00m;
        var amount = 1.5m; // Below threshold of 2.0

        // Act
        var result = strategy.CalculatePrice(basePrice, amount);

        // Assert
        // Base price: 5.00 * 1.5 = 7.50 (no discount)
        result.Should().Be(7.50m);
    }

    [Fact]
    public void CalculatePrice_WhenAmountEqualsThreshold_ShouldNotApplyDiscount()
    {
        // Arrange
        var baseStrategy = new PerKgPricingStrategy();
        var strategy = new DiscountedPricingStrategy(baseStrategy, 2.0m, 10.0m);
        var basePrice = 5.00m;
        var amount = 2.0m; // Exactly at threshold

        // Act
        var result = strategy.CalculatePrice(basePrice, amount);

        // Assert
        // Base price: 5.00 * 2.0 = 10.00 (no discount, threshold is > not >=)
        result.Should().Be(10.00m);
    }

    [Fact]
    public void GetUnit_ShouldReturnUnitFromBaseStrategy()
    {
        // Arrange
        var baseStrategy = new PerKgPricingStrategy();
        var strategy = new DiscountedPricingStrategy(baseStrategy, 2.0m, 10.0m);

        // Act
        var result = strategy.GetUnit();

        // Assert
        result.Should().Be("kg");
    }

    [Fact]
    public void Constructor_WithNullBaseStrategy_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var act = () => new DiscountedPricingStrategy(null!, 2.0m, 10.0m);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
