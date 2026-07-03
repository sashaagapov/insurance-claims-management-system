using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public class MockUser
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }

    // Demo users support role-sensitive MVP flows later. They are not real authentication accounts.
}
