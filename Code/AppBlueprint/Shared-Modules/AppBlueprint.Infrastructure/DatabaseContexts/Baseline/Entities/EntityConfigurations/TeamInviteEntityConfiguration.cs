using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

// NOTE: This is an old configuration - renamed to match the renamed entity
public sealed class OldTeamInviteEntityConfiguration : IEntityTypeConfiguration<OldTeamInviteEntity>
{
    public void Configure(EntityTypeBuilder<OldTeamInviteEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table Mapping - use different table name to avoid conflicts
        builder.ToTable("OldTeamInvites");

        // Primary Key        
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        // Handle SensitiveData attribute
        builder.Property(e => e.Email)
            .HasAnnotation("SensitiveData", true);

        // Relationships
        builder.HasOne(e => e.Team)
            .WithMany()
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Owner)
            .WithMany()
            .HasForeignKey("OwnerId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
