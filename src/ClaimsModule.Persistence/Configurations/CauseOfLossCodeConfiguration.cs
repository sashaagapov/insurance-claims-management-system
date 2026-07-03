using ClaimsModule.Domain.Entities;
using ClaimsModule.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public class CauseOfLossCodeConfiguration : IEntityTypeConfiguration<CauseOfLossCode>
{
    public void Configure(EntityTypeBuilder<CauseOfLossCode> builder)
    {
        builder.ToTable("CauseOfLossCodes");

        builder.HasKey(cause => cause.Id);

        builder.Property(cause => cause.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(cause => cause.Code)
            .IsUnique();

        builder.Property(cause => cause.Description)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasMany(cause => cause.Claims)
            .WithOne(claim => claim.CauseOfLossCode)
            .HasForeignKey(claim => claim.CauseOfLossCodeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.CauseOfLossCodes);
    }
}
