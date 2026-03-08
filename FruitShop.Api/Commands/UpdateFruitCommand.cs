using MediatR;
using FruitShop.Api.DTOs;

namespace FruitShop.Api.Commands;

/// <summary>
/// Command for updating an existing fruit.
/// </summary>
public class UpdateFruitCommand : IRequest<FruitDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string PricingStrategyType { get; set; } = string.Empty;
    public decimal? DiscountThreshold { get; set; }
    public decimal? DiscountPercentage { get; set; }
}
