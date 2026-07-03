using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Claims.GetClaimById;

public record RiskObjectDetailDto(
    Guid Id,
    RiskObjectType ObjectType,
    string? ExternalReference,
    string Description);
