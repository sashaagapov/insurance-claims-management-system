using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Application.Interfaces;

public interface IClaimsModuleDbContext
{
    DbSet<Policy> Policies { get; }
    DbSet<Claim> Claims { get; }
    DbSet<ClaimParty> ClaimParties { get; }
    DbSet<RiskObject> RiskObjects { get; }
    DbSet<AuditLogEntry> AuditLogEntries { get; }
    DbSet<CauseOfLossCode> CauseOfLossCodes { get; }
    DbSet<MockUser> MockUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
