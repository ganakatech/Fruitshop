using MediatR;
using FruitShop.Api.Services;

namespace FruitShop.Api.Queries;

/// <summary>
/// Handler for CalculateOrderTotalQuery.
/// </summary>
public class CalculateOrderTotalQueryHandler : IRequestHandler<CalculateOrderTotalQuery, decimal>
{
    private readonly IOrderService _orderService;

    public CalculateOrderTotalQueryHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<decimal> Handle(CalculateOrderTotalQuery request, CancellationToken cancellationToken)
    {
        return await _orderService.CalculateTotalAsync(request.Order);
    }
}
