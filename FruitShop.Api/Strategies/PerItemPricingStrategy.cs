namespace FruitShop.Api.Strategies;

/// <summary>
/// Pricing strategy that calculates price based on item count.
/// </summary>
public class PerItemPricingStrategy : IPricingStrategy
{
    /// <summary>
    /// Calculates the price by multiplying base price by item count.
    /// </summary>
    public decimal CalculatePrice(decimal basePrice, decimal amount)
    {
        return basePrice * amount;
    }

    /// <summary>
    /// Returns "item" as the unit of measurement.
    /// </summary>
    public string GetUnit() => "item";
}
