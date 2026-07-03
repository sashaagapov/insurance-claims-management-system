using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Reserves.RejectReserve;

public record RejectReserveResponse(
    Guid ReserveId,
    Guid ClaimId,
    decimal Amount,
    string Currency,
    ReserveStatus Status,
    Guid? RejectedByUserId,
    DateTime? RejectedAtUtc,
    string? RejectionReason);
