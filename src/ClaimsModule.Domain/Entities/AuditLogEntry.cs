namespace ClaimsModule.Domain.Entities;

public class AuditLogEntry
{
    public Guid Id { get; set; }
    public Guid ClaimId { get; set; }
    public Claim? Claim { get; set; }
    public string Action { get; set; } = string.Empty;
    public Guid ActorUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? Details { get; set; }

    // Audit records are append-only by intent. Future code should add new entries instead of editing history.
}
