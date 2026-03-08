using FruitShop.Api.Models;

namespace FruitShop.Api.Services;

/// <summary>
/// Service interface for fruit CRUD operations.
/// </summary>
public interface IFruitService
{
    Task<List<Fruit>> GetAllAsync();
    Task<Fruit?> GetByIdAsync(int id);
    Task<Fruit> CreateAsync(Fruit fruit);
    Task<Fruit> UpdateAsync(Fruit fruit);
    Task<bool> DeleteAsync(int id);
}
