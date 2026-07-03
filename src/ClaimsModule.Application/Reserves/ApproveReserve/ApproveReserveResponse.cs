using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Reserves.ApproveReserve;

public record ApproveReserveResponse(
    Guid ReserveId,
    Guid ClaimId,
    decimal Amount,
    string Currency,
    ReserveStatus Status,
    Guid? ApprovedByUserId,
    DateTime? ApprovedAtUtc);
