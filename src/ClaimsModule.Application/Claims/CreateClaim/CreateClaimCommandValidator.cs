using FluentValidation;

namespace ClaimsModule.Application.Claims.CreateClaim;

public class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator()
    {
        RuleFor(command => command.PolicyId)
            .NotEmpty();

        RuleFor(command => command.CauseOfLossCodeId)
            .NotEmpty();

        RuleFor(command => command.LossDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Loss date cannot be in the future.");

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(command => command.CreatedByUserId)
            .NotEmpty();

        RuleFor(command => command.Parties)
            .NotEmpty()
            .WithMessage("At least one claim party is required.");

        RuleForEach(command => command.Parties)
            .ChildRules(party =>
            {
                party.RuleFor(item => item.FullName)
                    .NotEmpty()
                    .MaximumLength(200);

                party.RuleFor(item => item.Email)
                    .MaximumLength(250);

                party.RuleFor(item => item.Phone)
                    .MaximumLength(50);

                party.RuleFor(item => item.Notes)
                    .MaximumLength(1000);
            });

        RuleFor(command => command.RiskObjects)
            .NotEmpty()
            .WithMessage("At least one risk object is required.");

        RuleForEach(command => command.RiskObjects)
            .ChildRules(riskObject =>
            {
                riskObject.RuleFor(item => item.ExternalReference)
                    .MaximumLength(100);

                riskObject.RuleFor(item => item.Description)
                    .NotEmpty()
                    .MaximumLength(1000);
            });
    }
}
