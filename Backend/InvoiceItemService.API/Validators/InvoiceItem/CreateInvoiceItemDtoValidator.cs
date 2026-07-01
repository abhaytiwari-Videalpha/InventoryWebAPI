using FluentValidation;
using InvoiceItemService.API.DTOs;

namespace InvoiceItemService.API.Validators;

public class CreateInvoiceItemDtoValidator
    : AbstractValidator<CreateInvoiceItemDto>
{
    public CreateInvoiceItemDtoValidator()
    {
        RuleFor(x => x.InvoiceId)
            .GreaterThan(0)
            .WithMessage("Valid Invoice ID is required.");

        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Valid Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than 0.");
    }
}