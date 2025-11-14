using AppBlueprint.Contracts.Baseline.Permissions.Requests;
using AppBlueprint.Contracts.Baseline.Role.Requests;
using FluentValidation;

namespace AppBlueprint.Application.Validations;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    private const int NameMaxLength = 100;
    private const int DescriptionMaxLength = 500;

    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Role name is required.")
            .MaximumLength(NameMaxLength)
            .WithMessage($"Role name must not exceed {NameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(DescriptionMaxLength)
            .WithMessage($"Role description must not exceed {DescriptionMaxLength} characters.")
            .When(x => x.Description is not null);

        // Validate each permission in the Permissions collection
        RuleForEach(x => x.Permissions)
            .SetValidator(new CreatePermissionRequestValidator())
            .When(x => x.Permissions is not null);
    }
}

public class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequest>
{
    private const int NameMaxLength = 100;
    private const int DescriptionMaxLength = 500;

    public CreatePermissionRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Permission name is required.")
            .MaximumLength(NameMaxLength)
            .WithMessage($"Permission name must not exceed {NameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(DescriptionMaxLength)
            .WithMessage($"Permission description must not exceed {DescriptionMaxLength} characters.")
            .When(x => x.Description is not null);
    }
}
