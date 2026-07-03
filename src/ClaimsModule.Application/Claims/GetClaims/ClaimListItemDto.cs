using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Claims.GetClaims;

public record ClaimListItemDto(
    Guid ClaimId,
    string ClaimNumber,
    string PolicyNumber,
    string PolicyholderName,
    string CauseOfLossCode,
    DateOnly LossDate,
    DateTime ReportedAtUtc,
    ClaimStatus Status,
    DateTime CreatedAtUtc);
