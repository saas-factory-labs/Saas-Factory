using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyMember;

/// <summary>
/// Entity configuration for FamilyMemberEntity defining table structure, relationships, and constraints.
/// </summary>
public sealed class FamilyMemberEntityConfiguration : BaseEntityConfiguration<FamilyMemberEntity>
{
    public override void Configure(EntityTypeBuilder<FamilyMemberEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
// Define table name
        builder.ToTable("FamilyMembers");

        // Primary Key
        builder.HasKey(fm => fm.Id);

        // Properties
        builder.Property(fm => fm.Alias)
            .HasMaxLength(100);

        builder.Property(fm => fm.IsActive)
            .IsRequired();        // Configure FirstName property (GDPR classification handled by data attribute)
        builder.Property(fm => fm.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        // Configure LastName property (GDPR classification handled by data attribute)
        builder.Property(fm => fm.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(fm => fm.User)
            .WithMany()
            .HasForeignKey(fm => fm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fm => fm.Family)
            .WithMany(f => f.FamilyMembers)
            .HasForeignKey(fm => fm.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance and constraints
        builder.HasIndex(fm => fm.UserId);
        builder.HasIndex(fm => fm.FamilyId);
        builder.HasIndex(fm => new { fm.FamilyId, fm.UserId }).IsUnique(); // Ensure unique family member per family
    }
}
