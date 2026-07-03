using ClaimsModule.Application.Interfaces;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence;

public class ClaimsModuleDbContext(DbContextOptions<ClaimsModuleDbContext> options)
    : DbContext(options), IClaimsModuleDbContext
{
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<ClaimParty> ClaimParties => Set<ClaimParty>();
    public DbSet<RiskObject> RiskObjects => Set<RiskObject>();
    public DbSet<Reserve> Reserves => Set<Reserve>();
    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();
    public DbSet<CauseOfLossCode> CauseOfLossCodes => Set<CauseOfLossCode>();
    public DbSet<MockUser> MockUsers => Set<MockUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClaimsModuleDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
