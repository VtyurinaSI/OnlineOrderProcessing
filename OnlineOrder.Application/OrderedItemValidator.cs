using FluentValidation;
using OnlineOrder.Application.DTOs;

namespace OnlineOrder.Application
{
    public class OrderedItemValidator : AbstractValidator<OrderedItemDto>
    {
        public OrderedItemValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Item name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be positive.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be positive.");
        }
    }
}
