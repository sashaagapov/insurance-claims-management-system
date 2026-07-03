using ClaimsModule.Application.Common;
using ClaimsModule.Domain.Enums;
using MediatR;

namespace ClaimsModule.Application.Claims.UpdateClaimStatus;

public record UpdateClaimStatusCommand(
    Guid ClaimId,
    ClaimStatus NewStatus,
    Guid ActorUserId)
    : IRequest<Result<UpdateClaimStatusResponse>>;
