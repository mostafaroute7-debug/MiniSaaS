
using FluentValidation;
using MiniSaaS.Application.Tenants.DTOs;

namespace MiniSaaS.Application.Tenants.Validators;

public sealed class CreateTenantValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tenant name is required.")
            .MaximumLength(100)
            .WithMessage("Tenant name must not exceed 100 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("Tenant slug is required.")
            .MaximumLength(100)
            .WithMessage("Tenant slug must not exceed 100 characters.");

        RuleFor(x => x.Slug)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage(
                "Tenant slug can contain lowercase letters, numbers and hyphens only.")
            .When(x => !string.IsNullOrWhiteSpace(x.Slug));
    }
}
