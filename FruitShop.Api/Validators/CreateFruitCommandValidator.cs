using FluentValidation;
using FruitShop.Api.Commands;

namespace FruitShop.Api.Validators;

/// <summary>
/// Validator for CreateFruitCommand.
/// </summary>
public class CreateFruitCommandValidator : AbstractValidator<CreateFruitCommand>
{
    public CreateFruitCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Fruit name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage("Base price must be greater than 0");

        RuleFor(x => x.PricingStrategyType)
            .NotEmpty().WithMessage("Pricing strategy type is required")
            .Must(BeValidPricingStrategyType).WithMessage("Pricing strategy type must be 'PerKg', 'PerItem', or 'Discounted'");

        When(x => x.PricingStrategyType == "Discounted", () =>
        {
            RuleFor(x => x.DiscountThreshold)
                .NotNull().WithMessage("Discount threshold is required for Discounted pricing strategy")
                .GreaterThan(0).WithMessage("Discount threshold must be greater than 0");

            RuleFor(x => x.DiscountPercentage)
                .NotNull().WithMessage("Discount percentage is required for Discounted pricing strategy")
                .GreaterThan(0).WithMessage("Discount percentage must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Discount percentage cannot exceed 100");
        });
    }

    private bool BeValidPricingStrategyType(string? pricingStrategyType)
    {
        if (string.IsNullOrWhiteSpace(pricingStrategyType))
        {
            return false;
        }

        return pricingStrategyType == "PerKg" || 
               pricingStrategyType == "PerItem" || 
               pricingStrategyType == "Discounted";
    }
}
