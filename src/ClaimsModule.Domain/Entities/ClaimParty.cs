using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public class ClaimParty
{
    public Guid Id { get; set; }
    public Guid ClaimId { get; set; }
    public Claim? Claim { get; set; }
    public PartyType PartyType { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
}
