using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims");

        builder.HasKey(claim => claim.Id);

        builder.Property(claim => claim.ClaimSequenceNumber)
            .ValueGeneratedOnAdd();

        builder.Property(claim => claim.ClaimNumber)
            .IsRequired()
            .HasMaxLength(50)
            .HasComputedColumnSql(
                "'CLM-' + CONVERT(char(4), DATEPART(year, [CreatedAtUtc])) + '-' + RIGHT('0000000' + CONVERT(varchar(7), [ClaimSequenceNumber]), 7)",
                stored: true);

        builder.HasIndex(claim => claim.ClaimNumber)
            .IsUnique();

        builder.Property(claim => claim.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(claim => claim.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne<MockUser>()
            .WithMany()
            .HasForeignKey(claim => claim.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
