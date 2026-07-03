using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Policies.GetPolicies;

public record PolicyListItemDto(
    Guid Id,
    string PolicyNumber,
    string PolicyholderName,
    string ProductType,
    DateOnly EffectiveFrom,
    DateOnly EffectiveTo,
    PolicyStatus Status,
    decimal CoverageLimit,
    string Currency);
