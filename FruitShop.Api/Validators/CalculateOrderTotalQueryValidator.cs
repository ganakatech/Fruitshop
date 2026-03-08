using FluentValidation;
using FruitShop.Api.Queries;

namespace FruitShop.Api.Validators;

/// <summary>
/// Validator for CalculateOrderTotalQuery.
/// </summary>
public class CalculateOrderTotalQueryValidator : AbstractValidator<CalculateOrderTotalQuery>
{
    public CalculateOrderTotalQueryValidator()
    {
        RuleFor(x => x.Order)
            .NotNull().WithMessage("Order is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.Order.Items)
                    .NotNull().WithMessage("Order items are required")
                    .NotEmpty().WithMessage("Order must contain at least one item");

                RuleForEach(x => x.Order.Items)
                    .ChildRules(item =>
                    {
                        item.RuleFor(i => i.FruitId)
                            .GreaterThan(0).WithMessage("Fruit ID must be greater than 0");

                        item.RuleFor(i => i.Amount)
                            .GreaterThan(0).WithMessage("Amount must be greater than 0");
                    });
            });
    }
}
