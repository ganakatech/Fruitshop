using MediatR;
using FruitShop.Api.Models;

namespace FruitShop.Api.Queries;

/// <summary>
/// Query for calculating the total price of an order.
/// </summary>
public class CalculateOrderTotalQuery : IRequest<decimal>
{
    public Order Order { get; set; } = new();
}
