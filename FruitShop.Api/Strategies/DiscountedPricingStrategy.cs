namespace FruitShop.Api.Strategies;

/// <summary>
/// Pricing strategy that wraps another strategy and applies a discount when a threshold is met.
/// </summary>
public class DiscountedPricingStrategy : IPricingStrategy
{
    private readonly IPricingStrategy _baseStrategy;
    private readonly decimal _discountThreshold;
    private readonly decimal _discountPercentage;

    /// <summary>
    /// Initializes a new instance of the DiscountedPricingStrategy.
    /// </summary>
    /// <param name="baseStrategy">The base pricing strategy to wrap.</param>
    /// <param name="discountThreshold">The threshold amount (weight or quantity) that triggers the discount.</param>
    /// <param name="discountPercentage">The discount percentage to apply (e.g., 10 for 10% off).</param>
    public DiscountedPricingStrategy(IPricingStrategy baseStrategy, decimal discountThreshold, decimal discountPercentage)
    {
        _baseStrategy = baseStrategy ?? throw new ArgumentNullException(nameof(baseStrategy));
        _discountThreshold = discountThreshold;
        _discountPercentage = discountPercentage;
    }

    /// <summary>
    /// Calculates the price using the base strategy and applies discount if threshold is met.
    /// </summary>
    public decimal CalculatePrice(decimal basePrice, decimal amount)
    {
        var price = _baseStrategy.CalculatePrice(basePrice, amount);
        
        if (amount > _discountThreshold)
        {
            var discount = price * (_discountPercentage / 100m);
            price -= discount;
        }
        
        return price;
    }

    /// <summary>
    /// Returns the unit from the base strategy.
    /// </summary>
    public string GetUnit() => _baseStrategy.GetUnit();
}
