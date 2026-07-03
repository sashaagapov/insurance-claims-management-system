using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("AuditLogEntries");

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(entry => entry.Details)
            .HasMaxLength(2000);

        builder.HasOne(entry => entry.Claim)
            .WithMany(claim => claim.AuditLogEntries)
            .HasForeignKey(entry => entry.ClaimId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MockUser>()
            .WithMany()
            .HasForeignKey(entry => entry.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entry => new { entry.ClaimId, entry.CreatedAtUtc });
    }
}
