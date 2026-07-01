using FluentValidation;
using InvoiceService.API.DTOs;

namespace InvoiceService.API.Validators;

public class CreateInvoiceDtoValidator
    : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("Valid Customer ID is required.");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage("Total amount must be greater than 0.");

        RuleFor(x => x.InvoiceDate)
            .NotEmpty()
            .WithMessage("Invoice date is required.");
    }
}