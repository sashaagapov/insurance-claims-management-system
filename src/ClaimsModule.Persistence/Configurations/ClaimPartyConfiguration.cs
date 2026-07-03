using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public class ClaimPartyConfiguration : IEntityTypeConfiguration<ClaimParty>
{
    public void Configure(EntityTypeBuilder<ClaimParty> builder)
    {
        builder.ToTable("ClaimParties");

        builder.HasKey(party => party.Id);

        builder.Property(party => party.PartyType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(party => party.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(party => party.Email)
            .HasMaxLength(250);

        builder.Property(party => party.Phone)
            .HasMaxLength(50);

        builder.Property(party => party.Notes)
            .HasMaxLength(1000);

        builder.HasOne(party => party.Claim)
            .WithMany(claim => claim.Parties)
            .HasForeignKey(party => party.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
