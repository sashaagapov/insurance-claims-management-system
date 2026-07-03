using FluentValidation;

namespace ClaimsModule.Application.Reserves.CreateReserve;

public class CreateReserveCommandValidator : AbstractValidator<CreateReserveCommand>
{
    public CreateReserveCommandValidator()
    {
        RuleFor(command => command.ClaimId)
            .NotEmpty();

        RuleFor(command => command.Amount)
            .GreaterThan(0);

        RuleFor(command => command.Currency)
            .NotEmpty()
            .Length(3)
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be a three-letter uppercase code, for example USD.");

        RuleFor(command => command.CreatedByUserId)
            .NotEmpty();
    }
}
