using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Claims.GetClaimById;

public record ReserveSummaryDto(
    Guid Id,
    decimal Amount,
    string Currency,
    ReserveStatus Status,
    DateTime CreatedAtUtc,
    Guid? ApprovedByUserId,
    DateTime? ApprovedAtUtc,
    Guid? RejectedByUserId,
    DateTime? RejectedAtUtc,
    DateTime? GlPostedAtUtc,
    string? GlPostingReference);
