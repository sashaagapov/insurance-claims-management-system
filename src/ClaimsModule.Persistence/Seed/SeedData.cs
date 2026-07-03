using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Persistence.Seed;

public static class SeedData
{
    public static IReadOnlyCollection<Policy> Policies =>
    [
        new()
        {
            Id = SeedDataIds.PolicyAuto,
            PolicyNumber = "POL-AUTO-1001",
            PolicyholderName = "Olena Petrenko",
            ProductType = "Auto",
            EffectiveFrom = new DateOnly(2026, 1, 1),
            EffectiveTo = new DateOnly(2026, 12, 31),
            Status = PolicyStatus.Active,
            CoverageLimit = 50000m,
            Currency = "USD"
        },
        new()
        {
            Id = SeedDataIds.PolicyHome,
            PolicyNumber = "POL-HOME-2001",
            PolicyholderName = "Dmytro Kovalenko",
            ProductType = "Home",
            EffectiveFrom = new DateOnly(2026, 2, 1),
            EffectiveTo = new DateOnly(2027, 1, 31),
            Status = PolicyStatus.Active,
            CoverageLimit = 150000m,
            Currency = "USD"
        },
        new()
        {
            Id = SeedDataIds.PolicyTravel,
            PolicyNumber = "POL-TRAVEL-3001",
            PolicyholderName = "Sasha Agapov",
            ProductType = "Travel",
            EffectiveFrom = new DateOnly(2026, 6, 1),
            EffectiveTo = new DateOnly(2026, 8, 31),
            Status = PolicyStatus.Active,
            CoverageLimit = 25000m,
            Currency = "USD"
        }
    ];

    public static IReadOnlyCollection<CauseOfLossCode> CauseOfLossCodes =>
    [
        new() { Id = SeedDataIds.CauseCollision, Code = "COLLISION", Description = "Collision or impact damage", IsActive = true },
        new() { Id = SeedDataIds.CauseTheft, Code = "THEFT", Description = "Theft or attempted theft", IsActive = true },
        new() { Id = SeedDataIds.CauseFire, Code = "FIRE", Description = "Fire or smoke damage", IsActive = true },
        new() { Id = SeedDataIds.CauseWaterDamage, Code = "WATER_DAMAGE", Description = "Water escape or flood damage", IsActive = true },
        new() { Id = SeedDataIds.CauseNaturalEvent, Code = "NATURAL_EVENT", Description = "Storm, hail, earthquake, or other natural event", IsActive = true }
    ];

    public static IReadOnlyCollection<MockUser> MockUsers =>
    [
        new() { Id = SeedDataIds.UserHandler, DisplayName = "Demo Handler", Role = UserRole.Handler, IsActive = true },
        new() { Id = SeedDataIds.UserSupervisor, DisplayName = "Demo Supervisor", Role = UserRole.Supervisor, IsActive = true },
        new() { Id = SeedDataIds.UserManager, DisplayName = "Demo Manager", Role = UserRole.Manager, IsActive = true }
    ];
}
