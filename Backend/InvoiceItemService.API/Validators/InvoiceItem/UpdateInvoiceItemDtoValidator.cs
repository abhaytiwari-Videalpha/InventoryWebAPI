using FluentValidation;
using InvoiceItemService.API.DTOs;

namespace InvoiceItemService.API.Validators;

public class UpdateInvoiceItemDtoValidator
    : AbstractValidator<UpdateInvoiceItemDto>
{
    public UpdateInvoiceItemDtoValidator()
    {
        RuleFor(x => x.InvoiceItemId)
            .GreaterThan(0)
            .WithMessage("Invalid Invoice Item ID.");

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