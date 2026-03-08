using MediatR;
using FruitShop.Api.DTOs;

namespace FruitShop.Api.Commands;

/// <summary>
/// Command for creating a new fruit.
/// </summary>
public class CreateFruitCommand : IRequest<FruitDto>
{
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string PricingStrategyType { get; set; } = string.Empty;
    public decimal? DiscountThreshold { get; set; }
    public decimal? DiscountPercentage { get; set; }
}
