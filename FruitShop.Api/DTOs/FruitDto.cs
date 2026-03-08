namespace FruitShop.Api.DTOs;

/// <summary>
/// Response DTO for fruit data (used in API responses).
/// </summary>
public class FruitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string PricingStrategyType { get; set; } = string.Empty;
    public decimal? DiscountThreshold { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public string Unit { get; set; } = string.Empty; // "kg" or "item" from strategy
}
