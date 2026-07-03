using ClaimsModule.Domain.Entities;
using ClaimsModule.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public class MockUserConfiguration : IEntityTypeConfiguration<MockUser>
{
    public void Configure(EntityTypeBuilder<MockUser> builder)
    {
        builder.ToTable("MockUsers");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.DisplayName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(user => user.Role)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasData(SeedData.MockUsers);
    }
}
