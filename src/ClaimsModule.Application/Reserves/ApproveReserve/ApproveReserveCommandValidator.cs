using FluentValidation;

namespace ClaimsModule.Application.Reserves.ApproveReserve;

public class ApproveReserveCommandValidator : AbstractValidator<ApproveReserveCommand>
{
    public ApproveReserveCommandValidator()
    {
        RuleFor(command => command.ClaimId)
            .NotEmpty();

        RuleFor(command => command.ReserveId)
            .NotEmpty();

        RuleFor(command => command.ActorUserId)
            .NotEmpty();
    }
}
