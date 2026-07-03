using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public class Reserve
{
    public Guid Id { get; set; }
    public Guid ClaimId { get; set; }
    public Claim? Claim { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public ReserveStatus Status { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    public Guid? RejectedByUserId { get; set; }
    public DateTime? RejectedAtUtc { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? GlPostedAtUtc { get; set; }
    public string? GlPostingReference { get; set; }
}
