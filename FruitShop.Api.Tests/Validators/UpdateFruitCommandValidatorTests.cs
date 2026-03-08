using FluentAssertions;
using FluentValidation.TestHelper;
using FruitShop.Api.Commands;
using FruitShop.Api.Validators;

namespace FruitShop.Api.Tests.Validators;

public class UpdateFruitCommandValidatorTests
{
    private readonly UpdateFruitCommandValidator _validator;

    public UpdateFruitCommandValidatorTests()
    {
        _validator = new UpdateFruitCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new UpdateFruitCommand
        {
            Id = 1,
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
    public void Validate_WithZeroId_ShouldFail()
    {
        // Arrange
        var command = new UpdateFruitCommand
        {
            Id = 0,
            Name = "Apple",
            BasePrice = 2.00m,
            PricingStrategyType = "PerKg"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldFail()
    {
        // Arrange
        var command = new UpdateFruitCommand
        {
            Id = 1,
            Name = "",
            BasePrice = 2.00m,
            PricingStrategyType = "PerKg"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
