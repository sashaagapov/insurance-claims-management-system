using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Claims.GetClaimById;

public record ClaimDetailDto(
    Guid ClaimId,
    string ClaimNumber,
    Guid PolicyId,
    string PolicyNumber,
    string PolicyholderName,
    string ProductType,
    Guid CauseOfLossCodeId,
    string CauseOfLossCode,
    string CauseOfLossDescription,
    DateOnly LossDate,
    DateTime ReportedAtUtc,
    string Description,
    ClaimStatus Status,
    Guid CreatedByUserId,
    IReadOnlyCollection<ClaimPartyDetailDto> Parties,
    IReadOnlyCollection<RiskObjectDetailDto> RiskObjects,
    IReadOnlyCollection<ReserveSummaryDto> Reserves,
    IReadOnlyCollection<AuditLogEntryDto> AuditLogEntries);
