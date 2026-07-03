using ClaimsModule.Application.Common;
using MediatR;

namespace ClaimsModule.Application.Reserves.ApproveReserve;

public record ApproveReserveCommand(
    Guid ClaimId,
    Guid ReserveId,
    Guid ActorUserId)
    : IRequest<Result<ApproveReserveResponse>>;
