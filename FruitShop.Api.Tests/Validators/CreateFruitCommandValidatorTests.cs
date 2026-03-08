using FluentAssertions;
using FluentValidation.TestHelper;
using FruitShop.Api.Commands;
using FruitShop.Api.Validators;

namespace FruitShop.Api.Tests.Validators;

public class CreateFruitCommandValidatorTests
{
    private readonly CreateFruitCommandValidator _validator;

    public CreateFruitCommandValidatorTests()
    {
        _validator = new CreateFruitCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateFruitCommand
        {
            Name = "Apple",
            BasePrice = 2.00m,
            PricingStrategyType = "PerKg"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldFail()
    {
        // Arrange
        var command = new CreateFruitCommand
        {
            Name = "",
            BasePrice = 2.00m,
            PricingStrategyType = "PerKg"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithZeroBasePrice_ShouldFail()
    {
        // Arrange
        var command = new CreateFruitCommand
        {
            Name = "Apple",
            BasePrice = 0m,
            PricingStrategyType = "PerKg"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BasePrice);
    }

    [Fact]
    public void Validate_WithInvalidPricingStrategyType_ShouldFail()
    {
        // Arrange
        var command = new CreateFruitCommand
        {
            Name = "Apple",
            BasePrice = 2.00m,
            PricingStrategyType = "Invalid"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PricingStrategyType);
    }

    [Fact]
    public void Validate_WithDiscountedStrategyAndMissingDiscountParams_ShouldFail()
    {
        // Arrange
        var command = new CreateFruitCommand
        {
            Name = "Cherry",
            BasePrice = 5.00m,
            PricingStrategyType = "Discounted"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DiscountThreshold);
        result.ShouldHaveValidationErrorFor(x => x.DiscountPercentage);
    }
}
