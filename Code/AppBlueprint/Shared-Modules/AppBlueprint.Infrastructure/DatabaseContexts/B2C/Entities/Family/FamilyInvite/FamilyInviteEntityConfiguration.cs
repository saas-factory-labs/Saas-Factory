using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyInvite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Family.FamilyInvite;

/// <summary>
/// Entity configuration for FamilyInviteEntity defining table structure, relationships, and constraints.
/// Manages family invitation system for B2C scenarios.
/// </summary>
public sealed class FamilyInviteEntityConfiguration : BaseEntityConfiguration<FamilyInviteEntity>
{
    public override void Configure(EntityTypeBuilder<FamilyInviteEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
// Table Mapping
        builder.ToTable("FamilyInvites");

        // Primary Key
        builder.HasKey(fi => fi.Id);

        // Configure ULID ID
        builder.Property(fi => fi.Id)
            .HasMaxLength(40)
            .IsRequired();

        // Configure BaseEntity properties
        builder.Property(fi => fi.CreatedAt)
            .IsRequired();

        builder.Property(fi => fi.LastUpdatedAt);

        builder.Property(fi => fi.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Properties
        builder.Property(fi => fi.FamilyId)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(fi => fi.UserId)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(fi => fi.ExpireAt)
            .IsRequired();

        builder.Property(fi => fi.IsActive)
            .IsRequired();

        // Relationships
        builder.HasOne(fi => fi.Family)
            .WithMany(f => f.FamilyInvites)
            .HasForeignKey(fi => fi.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fi => fi.Owner)
            .WithMany()
            .HasForeignKey(fi => fi.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(fi => fi.Id).IsUnique();
        builder.HasIndex(fi => fi.FamilyId);
        builder.HasIndex(fi => fi.UserId);
        builder.HasIndex(fi => fi.IsSoftDeleted);

        // Query filter for soft delete
        builder.HasQueryFilter(fi => !fi.IsSoftDeleted);
    }
}
