using ClaimsModule.Application.Common;
using MediatR;

namespace ClaimsModule.Application.Claims.CreateClaim;

public record CreateClaimCommand(
    Guid PolicyId,
    Guid CauseOfLossCodeId,
    DateOnly LossDate,
    string Description,
    Guid CreatedByUserId,
    IReadOnlyCollection<CreateClaimPartyDto> Parties,
    IReadOnlyCollection<CreateRiskObjectDto> RiskObjects)
    : IRequest<Result<CreateClaimResponse>>;
