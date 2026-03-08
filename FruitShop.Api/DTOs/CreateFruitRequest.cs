namespace FruitShop.Api.DTOs;

/// <summary>
/// Request DTO for creating a new fruit.
/// </summary>
public class CreateFruitRequest
{
    /// <summary>
    /// The name of the fruit.
    /// </summary>
    /// <example>Apple</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The base price of the fruit.
    /// </summary>
    /// <example>2.00</example>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// The pricing strategy type. Valid values are: "PerKg", "PerItem", or "Discounted".
    /// - "PerKg": Price is calculated per kilogram
    /// - "PerItem": Price is calculated per item
    /// - "Discounted": Price includes discount logic (requires DiscountThreshold and DiscountPercentage)
    /// </summary>
    /// <example>PerKg</example>
    public string PricingStrategyType { get; set; } = string.Empty;

    /// <summary>
    /// The discount threshold (required when PricingStrategyType is "Discounted").
    /// This is the minimum amount (in kg or items) required to trigger the discount.
    /// </summary>
    /// <example>2.0</example>
    public decimal? DiscountThreshold { get; set; }

    /// <summary>
    /// The discount percentage (required when PricingStrategyType is "Discounted").
    /// This is the percentage discount applied when the threshold is met (0-100).
    /// </summary>
    /// <example>10.0</example>
    public decimal? DiscountPercentage { get; set; }
}
