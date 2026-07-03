using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public class Claim
{
    public Guid Id { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    public Guid PolicyId { get; set; }
    public Policy? Policy { get; set; }
    public Guid CauseOfLossCodeId { get; set; }
    public CauseOfLossCode? CauseOfLossCode { get; set; }
    public DateOnly LossDate { get; set; }
    public DateTime ReportedAtUtc { get; set; }
    public string Description { get; set; } = string.Empty;
    public ClaimStatus Status { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<ClaimParty> Parties { get; set; } = new List<ClaimParty>();
    public ICollection<RiskObject> RiskObjects { get; set; } = new List<RiskObject>();
    public ICollection<Reserve> Reserves { get; set; } = new List<Reserve>();
    public ICollection<AuditLogEntry> AuditLogEntries { get; set; } = new List<AuditLogEntry>();
}
