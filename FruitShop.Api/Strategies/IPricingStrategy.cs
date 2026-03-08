namespace FruitShop.Api.Strategies;

/// <summary>
/// Interface defining the contract for pricing strategies.
/// </summary>
public interface IPricingStrategy
{
    /// <summary>
    /// Calculates the price based on base price and amount (weight or quantity).
    /// </summary>
    /// <param name="basePrice">The base price per unit.</param>
    /// <param name="amount">The amount (weight in kg or quantity of items).</param>
    /// <returns>The calculated total price.</returns>
    decimal CalculatePrice(decimal basePrice, decimal amount);

    /// <summary>
    /// Gets the unit of measurement for this pricing strategy.
    /// </summary>
    /// <returns>"kg" for weight-based pricing or "item" for quantity-based pricing.</returns>
    string GetUnit();
}
