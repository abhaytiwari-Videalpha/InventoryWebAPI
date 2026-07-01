using FluentValidation;
using IdentityService.API.Auth.DTOs;

namespace IdentityService.API.Validators;

public class LoginDtoValidator
    : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}