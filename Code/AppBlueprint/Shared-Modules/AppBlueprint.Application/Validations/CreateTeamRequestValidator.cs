using AppBlueprint.Contracts.B2B.Contracts.Team.Requests;
using FluentValidation;

namespace AppBlueprint.Application.Validations;

public class CreateTeamRequestValidator : AbstractValidator<CreateTeamRequest>
{
    private const int NameMaxLength = 100;
    private const int DescriptionMaxLength = 500;

    public CreateTeamRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Team name is required.")
            .MaximumLength(NameMaxLength)
            .WithMessage($"Team name must not exceed {NameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(DescriptionMaxLength)
            .WithMessage($"Team description must not exceed {DescriptionMaxLength} characters.")
            .When(x => x.Description is not null);
    }
}
