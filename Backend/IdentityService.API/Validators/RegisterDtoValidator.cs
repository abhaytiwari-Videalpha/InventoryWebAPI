using FluentValidation;
using IdentityService.API.Auth.DTOs;

namespace IdentityService.API.Validators;

public class RegisterDtoValidator
    : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}