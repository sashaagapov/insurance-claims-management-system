using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public class RiskObjectConfiguration : IEntityTypeConfiguration<RiskObject>
{
    public void Configure(EntityTypeBuilder<RiskObject> builder)
    {
        builder.ToTable("RiskObjects");

        builder.HasKey(riskObject => riskObject.Id);

        builder.Property(riskObject => riskObject.ObjectType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(riskObject => riskObject.ExternalReference)
            .HasMaxLength(100);

        builder.Property(riskObject => riskObject.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(riskObject => riskObject.Claim)
            .WithMany(claim => claim.RiskObjects)
            .HasForeignKey(riskObject => riskObject.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
