
using FluentValidation;
using MiniSaaS.Application.Tenants.DTOs;

namespace MiniSaaS.Application.Tenants.Validators;

public sealed class CreateTenantValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug must contain lowercase letters, numbers and hyphens only.");
    }
}
