using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Claims.GetClaimById;

public record ClaimPartyDetailDto(
    Guid Id,
    PartyType PartyType,
    string FullName,
    string? Email,
    string? Phone,
    string? Notes);
