using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Models;
using FruitShop.Api.Data.EntityConfigurations;

namespace FruitShop.Api.Data;

/// <summary>
/// Entity Framework Core DbContext for the Fruit Shop application.
/// </summary>
public class FruitShopDbContext : DbContext
{
    public DbSet<Fruit> Fruits { get; set; }
    
    public FruitShopDbContext(DbContextOptions<FruitShopDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new FruitConfiguration());
    }
}
