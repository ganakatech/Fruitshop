using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Data;
using FruitShop.Api.Models;
using FruitShop.Api.Factories;

namespace FruitShop.Api.Services;

/// <summary>
/// Service for managing fruit CRUD operations using Entity Framework Core.
/// </summary>
public class FruitService : IFruitService
{
    private readonly FruitShopDbContext _context;

    public FruitService(FruitShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<Fruit>> GetAllAsync()
    {
        var fruits = await _context.Fruits.ToListAsync();
        return fruits.Select(FruitFactory.ReconstructStrategy).ToList();
    }

    public async Task<Fruit?> GetByIdAsync(int id)
    {
        var fruit = await _context.Fruits.FindAsync(id);
        if (fruit == null)
        {
            return null;
        }
        
        return FruitFactory.ReconstructStrategy(fruit);
    }

    public async Task<Fruit> CreateAsync(Fruit fruit)
    {
        _context.Fruits.Add(fruit);
        await _context.SaveChangesAsync();
        return FruitFactory.ReconstructStrategy(fruit);
    }

    public async Task<Fruit> UpdateAsync(Fruit fruit)
    {
        var existingFruit = await _context.Fruits.FindAsync(fruit.Id);
        if (existingFruit == null)
        {
            throw new ArgumentException($"Fruit with ID {fruit.Id} not found.");
        }

        existingFruit.Name = fruit.Name;
        existingFruit.BasePrice = fruit.BasePrice;
        existingFruit.PricingStrategyType = fruit.PricingStrategyType;
        existingFruit.DiscountThreshold = fruit.DiscountThreshold;
        existingFruit.DiscountPercentage = fruit.DiscountPercentage;

        await _context.SaveChangesAsync();
        return FruitFactory.ReconstructStrategy(existingFruit);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var fruit = await _context.Fruits.FindAsync(id);
        if (fruit == null)
        {
            return false;
        }

        _context.Fruits.Remove(fruit);
        await _context.SaveChangesAsync();
        return true;
    }
}
