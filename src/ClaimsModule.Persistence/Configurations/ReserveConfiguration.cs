using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public class ReserveConfiguration : IEntityTypeConfiguration<Reserve>
{
    public void Configure(EntityTypeBuilder<Reserve> builder)
    {
        builder.ToTable("Reserves");

        builder.HasKey(reserve => reserve.Id);

        builder.Property(reserve => reserve.Amount)
            .HasPrecision(18, 2);

        builder.Property(reserve => reserve.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(reserve => reserve.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(reserve => reserve.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(reserve => reserve.GlPostingReference)
            .HasMaxLength(100);

        builder.HasOne(reserve => reserve.Claim)
            .WithMany(claim => claim.Reserves)
            .HasForeignKey(reserve => reserve.ClaimId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MockUser>()
            .WithMany()
            .HasForeignKey(reserve => reserve.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MockUser>()
            .WithMany()
            .HasForeignKey(reserve => reserve.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MockUser>()
            .WithMany()
            .HasForeignKey(reserve => reserve.RejectedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
