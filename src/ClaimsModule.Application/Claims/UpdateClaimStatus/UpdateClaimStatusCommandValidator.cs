using FluentValidation;

namespace ClaimsModule.Application.Claims.UpdateClaimStatus;

public class UpdateClaimStatusCommandValidator : AbstractValidator<UpdateClaimStatusCommand>
{
    public UpdateClaimStatusCommandValidator()
    {
        RuleFor(command => command.ClaimId)
            .NotEmpty();

        RuleFor(command => command.NewStatus)
            .IsInEnum();

        RuleFor(command => command.ActorUserId)
            .NotEmpty();
    }
}
