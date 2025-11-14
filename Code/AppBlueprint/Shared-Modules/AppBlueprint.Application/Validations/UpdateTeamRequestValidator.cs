using AppBlueprint.Contracts.B2B.Contracts.Team.Requests;
using FluentValidation;

namespace AppBlueprint.Application.Validations;

public class UpdateTeamRequestValidator : AbstractValidator<UpdateTeamRequest>
{
    private const int NameMaxLength = 100;
    private const int DescriptionMaxLength = 500;

    public UpdateTeamRequestValidator()
    {
        // At least one property must be provided for update
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Name) || x.Description is not null)
            .WithMessage("At least one property (Name or Description) must be provided for update.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Team name cannot be empty if provided.")
            .MaximumLength(NameMaxLength)
            .WithMessage($"Team name must not exceed {NameMaxLength} characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(DescriptionMaxLength)
            .WithMessage($"Team description must not exceed {DescriptionMaxLength} characters.")
            .When(x => x.Description is not null);
    }
}
