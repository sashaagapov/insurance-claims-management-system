using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Claims.CreateClaim;

public record CreateClaimResponse(
    Guid ClaimId,
    string ClaimNumber,
    ClaimStatus Status,
    DateTime ReportedAtUtc);
