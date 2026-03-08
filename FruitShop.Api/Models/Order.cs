namespace FruitShop.Api.Models;

/// <summary>
/// DTO representing an order with multiple items.
/// </summary>
public class Order
{
    /// <summary>
    /// List of items in the order. Each item must have a valid FruitId and Amount.
    /// </summary>
    /// <example>
    /// [
    ///   { "fruitId": 1, "amount": 2.5 },
    ///   { "fruitId": 2, "amount": 10 }
    /// ]
    /// </example>
    public List<OrderItem> Items { get; set; } = new();
}
