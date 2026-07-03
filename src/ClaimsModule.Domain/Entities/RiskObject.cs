using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public class RiskObject
{
    public Guid Id { get; set; }
    public Guid ClaimId { get; set; }
    public Claim? Claim { get; set; }
    public RiskObjectType ObjectType { get; set; }
    public string? ExternalReference { get; set; }
    public string Description { get; set; } = string.Empty;
}
