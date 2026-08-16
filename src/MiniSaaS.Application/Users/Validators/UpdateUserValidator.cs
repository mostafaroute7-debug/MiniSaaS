
using FluentValidation;
using MiniSaaS.Application.Users.DTOs;
using MiniSaaS.Domain.Enums;

namespace MiniSaaS.Application.Users.Validators;

public sealed class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{

    public UpdateUserValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Role)
            .IsInEnum()
            .Must(role =>
                role == UserRole.Admin ||
                role == UserRole.Member)
            .WithMessage(
                "Role must be either Admin or Member.");

    }
}
