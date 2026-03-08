using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Data;
using FruitShop.Api.Models;
using FruitShop.Api.Factories;

namespace FruitShop.Api.Services;

/// <summary>
/// Service for calculating order totals.
/// </summary>
public class OrderService : IOrderService
{
    private readonly FruitShopDbContext _context;

    public OrderService(FruitShopDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> CalculateTotalAsync(Order order)
    {
        decimal total = 0;

        foreach (var item in order.Items)
        {
            var fruit = await _context.Fruits.FindAsync(item.FruitId);
            if (fruit == null)
            {
                throw new ArgumentException($"Fruit with ID {item.FruitId} not found.");
            }

            var fruitWithStrategy = FruitFactory.ReconstructStrategy(fruit);
            
            if (fruitWithStrategy.PricingStrategy == null)
            {
                throw new InvalidOperationException($"Pricing strategy not found for fruit {fruit.Name}.");
            }

            var itemPrice = fruitWithStrategy.PricingStrategy.CalculatePrice(
                fruitWithStrategy.BasePrice, 
                item.Amount);
            
            total += itemPrice;
        }

        return total;
    }
}
