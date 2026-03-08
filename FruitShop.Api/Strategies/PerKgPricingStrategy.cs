namespace FruitShop.Api.Strategies;

/// <summary>
/// Pricing strategy that calculates price based on weight in kilograms.
/// </summary>
public class PerKgPricingStrategy : IPricingStrategy
{
    /// <summary>
    /// Calculates the price by multiplying base price by weight in kg.
    /// </summary>
    public decimal CalculatePrice(decimal basePrice, decimal amount)
    {
        return basePrice * amount;
    }

    /// <summary>
    /// Returns "kg" as the unit of measurement.
    /// </summary>
    public string GetUnit() => "kg";
}
