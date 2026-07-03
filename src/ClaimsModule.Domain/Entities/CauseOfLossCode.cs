namespace ClaimsModule.Domain.Entities;

public class CauseOfLossCode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public ICollection<Claim> Claims { get; set; } = new List<Claim>();
}
