using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FruitShop.Api.Models;

namespace FruitShop.Api.Data.EntityConfigurations;

/// <summary>
/// Entity configuration for the Fruit entity.
/// </summary>
public class FruitConfiguration : IEntityTypeConfiguration<Fruit>
{
    public void Configure(EntityTypeBuilder<Fruit> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(f => f.BasePrice)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(f => f.PricingStrategyType)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(f => f.DiscountThreshold)
            .HasPrecision(18, 2);
        
        builder.Property(f => f.DiscountPercentage)
            .HasPrecision(5, 2);
    }
}
