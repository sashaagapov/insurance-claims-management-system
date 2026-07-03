using MediatR;

namespace ClaimsModule.Application.CauseOfLossCodes.GetCauseOfLossCodes;

public record GetCauseOfLossCodesQuery : IRequest<IReadOnlyCollection<CauseOfLossCodeDto>>;
