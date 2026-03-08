using FruitShop.Api.Models;

namespace FruitShop.Api.Services;

/// <summary>
/// Service interface for order price calculation.
/// </summary>
public interface IOrderService
{
    Task<decimal> CalculateTotalAsync(Order order);
}
