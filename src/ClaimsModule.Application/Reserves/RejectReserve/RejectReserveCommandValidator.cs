using FluentValidation;

namespace ClaimsModule.Application.Reserves.RejectReserve;

public class RejectReserveCommandValidator : AbstractValidator<RejectReserveCommand>
{
    public RejectReserveCommandValidator()
    {
        RuleFor(command => command.ClaimId)
            .NotEmpty();

        RuleFor(command => command.ReserveId)
            .NotEmpty();

        RuleFor(command => command.ActorUserId)
            .NotEmpty();

        RuleFor(command => command.Reason)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
