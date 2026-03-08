using FluentAssertions;
using FluentValidation.TestHelper;
using FruitShop.Api.Queries;
using FruitShop.Api.Models;
using FruitShop.Api.Validators;

namespace FruitShop.Api.Tests.Validators;

public class CalculateOrderTotalQueryValidatorTests
{
    private readonly CalculateOrderTotalQueryValidator _validator;

    public CalculateOrderTotalQueryValidatorTests()
    {
        _validator = new CalculateOrderTotalQueryValidator();
    }

    [Fact]
    public void Validate_WithValidOrder_ShouldPass()
    {
        // Arrange
        var query = new CalculateOrderTotalQuery
        {
            Order = new Order
            {
                Items = new List<OrderItem>
                {
                    new OrderItem { FruitId = 1, Amount = 2.0m }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNullOrder_ShouldFail()
    {
        // Arrange
        var query = new CalculateOrderTotalQuery
        {
            Order = null!
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Order);
    }

    [Fact]
    public void Validate_WithEmptyItems_ShouldFail()
    {
        // Arrange
        var query = new CalculateOrderTotalQuery
        {
            Order = new Order
            {
                Items = new List<OrderItem>()
            }
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Order.Items);
    }

    [Fact]
    public void Validate_WithInvalidFruitId_ShouldFail()
    {
        // Arrange
        var query = new CalculateOrderTotalQuery
        {
            Order = new Order
            {
                Items = new List<OrderItem>
                {
                    new OrderItem { FruitId = 0, Amount = 2.0m }
                }
            }
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor("Order.Items[0].FruitId");
    }
}
