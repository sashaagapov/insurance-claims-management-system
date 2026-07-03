using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClaimsModule.Persistence.Seed;

namespace ClaimsModule.Persistence.Configurations;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policies");

        builder.HasKey(policy => policy.Id);

        builder.Property(policy => policy.PolicyNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(policy => policy.PolicyNumber)
            .IsUnique();

        builder.Property(policy => policy.PolicyholderName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(policy => policy.ProductType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(policy => policy.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(policy => policy.CoverageLimit)
            .HasPrecision(18, 2);

        builder.Property(policy => policy.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.HasMany(policy => policy.Claims)
            .WithOne(claim => claim.Policy)
            .HasForeignKey(claim => claim.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.Policies);
    }
}
