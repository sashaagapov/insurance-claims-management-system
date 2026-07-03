using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Claims.CreateClaim;

public record CreateClaimPartyDto(
    PartyType PartyType,
    string FullName,
    string? Email,
    string? Phone,
    string? Notes);
