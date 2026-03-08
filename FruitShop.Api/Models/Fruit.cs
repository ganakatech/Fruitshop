using System.ComponentModel.DataAnnotations.Schema;
using FruitShop.Api.Strategies;

namespace FruitShop.Api.Models;

/// <summary>
/// Entity representing a fruit with pricing information.
/// </summary>
public class Fruit
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string PricingStrategyType { get; set; } = string.Empty; // "PerKg", "PerItem", "Discounted"
    public decimal? DiscountThreshold { get; set; } // For discounted strategy (e.g., 2.0 kg)
    public decimal? DiscountPercentage { get; set; } // For discounted strategy (e.g., 10.0)
    
    /// <summary>
    /// Not persisted - reconstructed from PricingStrategyType using factory.
    /// </summary>
    [NotMapped]
    public IPricingStrategy? PricingStrategy { get; set; }
}
