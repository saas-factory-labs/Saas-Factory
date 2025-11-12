using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for FamilyEntity defining table structure, relationships, and constraints.
/// Supports B2C family management and organization.
/// </summary>
public sealed class FamilyEntityConfiguration : BaseEntityConfiguration<FamilyEntity>
{
    public override void Configure(EntityTypeBuilder<FamilyEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
// Table mapping
        builder.ToTable("Families");

        // Primary Key
        builder.HasKey(f => f.Id);

        // Properties
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Description)
            .HasMaxLength(500);

        builder.Property(f => f.IsActive)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.LastUpdatedAt);

        // Relationships
        builder.HasOne(f => f.Owner)
            .WithMany()
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.FamilyMembers)
            .WithOne()
            .HasForeignKey(fm => fm.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.FamilyInvites)
            .WithOne()
            .HasForeignKey(fi => fi.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => f.Name)
            .IsUnique();
    }
}
