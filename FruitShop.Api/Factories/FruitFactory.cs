using FruitShop.Api.Models;
using FruitShop.Api.Strategies;

namespace FruitShop.Api.Factories;

/// <summary>
/// Factory for creating fruit instances with appropriate pricing strategies.
/// </summary>
public static class FruitFactory
{
    /// <summary>
    /// Creates a fruit instance with the appropriate pricing strategy based on the pricing type.
    /// </summary>
    /// <param name="name">The name of the fruit.</param>
    /// <param name="basePrice">The base price per unit.</param>
    /// <param name="pricingType">The type of pricing strategy ("PerKg", "PerItem", "Discounted").</param>
    /// <param name="discountThreshold">Optional discount threshold for discounted pricing.</param>
    /// <param name="discountPercentage">Optional discount percentage for discounted pricing.</param>
    /// <returns>A Fruit instance with the appropriate pricing strategy configured.</returns>
    public static Fruit Create(string name, decimal basePrice, string pricingType, 
        decimal? discountThreshold = null, decimal? discountPercentage = null)
    {
        IPricingStrategy strategy = pricingType switch
        {
            "PerKg" => new PerKgPricingStrategy(),
            "PerItem" => new PerItemPricingStrategy(),
            "Discounted" => CreateDiscountedStrategy(discountThreshold, discountPercentage),
            _ => throw new ArgumentException($"Unknown pricing strategy type: {pricingType}", nameof(pricingType))
        };

        var fruit = new Fruit
        {
            Name = name,
            BasePrice = basePrice,
            PricingStrategyType = pricingType,
            DiscountThreshold = discountThreshold,
            DiscountPercentage = discountPercentage,
            PricingStrategy = strategy
        };

        return fruit;
    }

    /// <summary>
    /// Reconstructs the pricing strategy for a fruit entity loaded from the database.
    /// </summary>
    /// <param name="fruit">The fruit entity to reconstruct the strategy for.</param>
    /// <returns>The fruit with its pricing strategy reconstructed.</returns>
    public static Fruit ReconstructStrategy(Fruit fruit)
    {
        if (fruit.PricingStrategy != null)
        {
            return fruit; // Already reconstructed
        }

        IPricingStrategy strategy = fruit.PricingStrategyType switch
        {
            "PerKg" => new PerKgPricingStrategy(),
            "PerItem" => new PerItemPricingStrategy(),
            "Discounted" => CreateDiscountedStrategy(fruit.DiscountThreshold, fruit.DiscountPercentage),
            _ => throw new ArgumentException($"Unknown pricing strategy type: {fruit.PricingStrategyType}")
        };

        fruit.PricingStrategy = strategy;
        return fruit;
    }

    private static IPricingStrategy CreateDiscountedStrategy(decimal? discountThreshold, decimal? discountPercentage)
    {
        if (!discountThreshold.HasValue || !discountPercentage.HasValue)
        {
            throw new ArgumentException("Discount threshold and percentage are required for Discounted pricing strategy.");
        }

        // Discounted strategy wraps PerKg strategy by default
        var baseStrategy = new PerKgPricingStrategy();
        return new DiscountedPricingStrategy(baseStrategy, discountThreshold.Value, discountPercentage.Value);
    }
}
