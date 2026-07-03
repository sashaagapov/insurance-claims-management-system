using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public class Policy
{
    public Guid Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string PolicyholderName { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly EffectiveTo { get; set; }
    public PolicyStatus Status { get; set; }
    public decimal CoverageLimit { get; set; }
    public string Currency { get; set; } = "USD";

    public ICollection<Claim> Claims { get; set; } = new List<Claim>();
}
