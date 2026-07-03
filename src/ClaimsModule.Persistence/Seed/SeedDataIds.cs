namespace ClaimsModule.Persistence.Seed;

public static class SeedDataIds
{
    public static readonly Guid PolicyAuto = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid PolicyHome = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid PolicyTravel = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid CauseCollision = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    public static readonly Guid CauseTheft = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");
    public static readonly Guid CauseFire = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003");
    public static readonly Guid CauseWaterDamage = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004");
    public static readonly Guid CauseNaturalEvent = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005");

    public static readonly Guid UserHandler = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000001");
    public static readonly Guid UserSupervisor = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000002");
    public static readonly Guid UserManager = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000003");
}
