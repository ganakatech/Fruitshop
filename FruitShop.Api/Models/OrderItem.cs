namespace FruitShop.Api.Models;

/// <summary>
/// DTO representing an item in an order with fruit reference and amount (unified field).
/// </summary>
public class OrderItem
{
    /// <summary>
    /// The ID of the fruit. Must be a valid fruit ID that exists in the system.
    /// Use GET /fruits to retrieve available fruit IDs.
    /// </summary>
    /// <example>1</example>
    public int FruitId { get; set; }

    /// <summary>
    /// The amount of fruit ordered. This field represents different units depending on the fruit's pricing strategy:
    /// - For "PerKg" pricing: Amount represents weight in kilograms (e.g., 2.5 for 2.5 kg)
    /// - For "PerItem" pricing: Amount represents quantity as a number of items (e.g., 10 for 10 items)
    /// - For "Discounted" pricing: Amount represents weight in kilograms (e.g., 3.0 for 3.0 kg)
    /// 
    /// The pricing strategy for each fruit can be checked using GET /fruits/{id} which returns the "unit" field.
    /// </summary>
    /// <example>2.5</example>
    public decimal Amount { get; set; }
}
