using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Claims.UpdateClaimStatus;

public record UpdateClaimStatusResponse(
    Guid ClaimId,
    string ClaimNumber,
    ClaimStatus OldStatus,
    ClaimStatus NewStatus,
    DateTime ChangedAtUtc);
