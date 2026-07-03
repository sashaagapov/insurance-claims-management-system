using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Claims.CreateClaim;

public record CreateRiskObjectDto(
    RiskObjectType ObjectType,
    string? ExternalReference,
    string Description);
