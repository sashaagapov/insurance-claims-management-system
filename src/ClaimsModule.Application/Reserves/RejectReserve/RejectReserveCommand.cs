using ClaimsModule.Application.Common;
using MediatR;

namespace ClaimsModule.Application.Reserves.RejectReserve;

public record RejectReserveCommand(
    Guid ClaimId,
    Guid ReserveId,
    Guid ActorUserId,
    string Reason)
    : IRequest<Result<RejectReserveResponse>>;
