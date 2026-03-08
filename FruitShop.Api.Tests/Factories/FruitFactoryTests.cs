using FluentAssertions;
using FruitShop.Api.Factories;
using FruitShop.Api.Strategies;

namespace FruitShop.Api.Tests.Factories;

public class FruitFactoryTests
{
    [Fact]
    public void Create_WithPerKgStrategy_ShouldCreateFruitWithPerKgPricingStrategy()
    {
        // Arrange
        var name = "Apple";
        var basePrice = 2.00m;
        var pricingType = "PerKg";

        // Act
        var fruit = FruitFactory.Create(name, basePrice, pricingType);

        // Assert
        fruit.Name.Should().Be(name);
        fruit.BasePrice.Should().Be(basePrice);
        fruit.PricingStrategyType.Should().Be(pricingType);
        fruit.PricingStrategy.Should().BeOfType<PerKgPricingStrategy>();
        fruit.PricingStrategy!.GetUnit().Should().Be("kg");
    }

    [Fact]
    public void Create_WithPerItemStrategy_ShouldCreateFruitWithPerItemPricingStrategy()
    {
        // Arrange
        var name = "Banana";
        var basePrice = 0.30m;
        var pricingType = "PerItem";

        // Act
        var fruit = FruitFactory.Create(name, basePrice, pricingType);

        // Assert
        fruit.Name.Should().Be(name);
        fruit.BasePrice.Should().Be(basePrice);
        fruit.PricingStrategyType.Should().Be(pricingType);
        fruit.PricingStrategy.Should().BeOfType<PerItemPricingStrategy>();
        fruit.PricingStrategy!.GetUnit().Should().Be("item");
    }

    [Fact]
    public void Create_WithDiscountedStrategy_ShouldCreateFruitWithDiscountedPricingStrategy()
    {
        // Arrange
        var name = "Cherry";
        var basePrice = 5.00m;
        var pricingType = "Discounted";
        var discountThreshold = 2.0m;
        var discountPercentage = 10.0m;

        // Act
        var fruit = FruitFactory.Create(name, basePrice, pricingType, discountThreshold, discountPercentage);

        // Assert
        fruit.Name.Should().Be(name);
        fruit.BasePrice.Should().Be(basePrice);
        fruit.PricingStrategyType.Should().Be(pricingType);
        fruit.DiscountThreshold.Should().Be(discountThreshold);
        fruit.DiscountPercentage.Should().Be(discountPercentage);
        fruit.PricingStrategy.Should().BeOfType<DiscountedPricingStrategy>();
    }

    [Fact]
    public void Create_WithInvalidPricingType_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "Test";
        var basePrice = 1.00m;
        var pricingType = "Invalid";

        // Act
        var act = () => FruitFactory.Create(name, basePrice, pricingType);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ReconstructStrategy_WithPerKgFruit_ShouldReconstructPerKgStrategy()
    {
        // Arrange
        var fruit = new Models.Fruit
        {
            Id = 1,
            Name = "Apple",
            BasePrice = 2.00m,
            PricingStrategyType = "PerKg"
        };

        // Act
        var result = FruitFactory.ReconstructStrategy(fruit);

        // Assert
        result.PricingStrategy.Should().BeOfType<PerKgPricingStrategy>();
        result.PricingStrategy!.GetUnit().Should().Be("kg");
    }

    [Fact]
    public void ReconstructStrategy_WithDiscountedFruit_ShouldReconstructDiscountedStrategy()
    {
        // Arrange
        var fruit = new Models.Fruit
        {
            Id = 1,
            Name = "Cherry",
            BasePrice = 5.00m,
            PricingStrategyType = "Discounted",
            DiscountThreshold = 2.0m,
            DiscountPercentage = 10.0m
        };

        // Act
        var result = FruitFactory.ReconstructStrategy(fruit);

        // Assert
        result.PricingStrategy.Should().BeOfType<DiscountedPricingStrategy>();
    }
}
