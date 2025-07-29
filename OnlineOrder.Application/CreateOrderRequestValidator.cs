using FluentValidation;
using OnlineOrder.Application.DTOs;

namespace OnlineOrder.Application
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Items)
                .NotNull().WithMessage("Items collection cannot be null.")
                .Must(i => i?.Count > 0).WithMessage("At least one item is required.");

            RuleForEach(x => x.Items).SetValidator(new OrderedItemValidator());
        }
    }
}
