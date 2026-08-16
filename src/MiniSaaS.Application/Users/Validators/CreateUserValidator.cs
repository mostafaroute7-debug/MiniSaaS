using FluentValidation;
using MiniSaaS.Application.Users.DTOs;
using MiniSaaS.Domain.Enums;

namespace MiniSaaS.Application.Users.Validators;

public sealed class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("User full name is required.")
            .MaximumLength(150)
            .WithMessage("User full name must not exceed 150 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("User email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.")
            .MaximumLength(255)
            .WithMessage("User email must not exceed 255 characters.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid user role.");
    }
}
