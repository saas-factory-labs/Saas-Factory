using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailInvite;

/// <summary>
/// Entity configuration for EmailInviteEntity defining table structure, relationships, and constraints.
/// Manages email invitation system and token validation.
/// </summary>
public sealed class EmailInviteEntityConfiguration : BaseEntityConfiguration<EmailInviteEntity>
{
    public override void Configure(EntityTypeBuilder<EmailInviteEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
builder.ToTable("EmailInvites");

        builder.HasKey(e => e.Id);

        // Configure ULID ID with proper length for prefixed ULID
        builder.Property(e => e.Id)
            .HasMaxLength(40)
            .IsRequired();

        // Configure foreign key property
        builder.Property(e => e.UserEntityId)
            .HasMaxLength(40);

        // BaseEntity properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt);

        builder.Property(e => e.IsSoftDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(e => e.ReferredEmailAddress)
            .IsRequired()
            .HasMaxLength(255)
            .HasAnnotation("SensitiveData", true);

        builder.Property(e => e.ExpireAt)
            .IsRequired();

        builder.Property(e => e.InviteIsUsed)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithMany(u => u.ReferralInvitations)
            .HasForeignKey(e => e.UserEntityId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_EmailInvites_Users_UserEntityId");

        // Indexes for performance
        builder.HasIndex(e => e.Token)
            .IsUnique()
            .HasDatabaseName("IX_EmailInvites_Token");

        builder.HasIndex(e => e.ReferredEmailAddress)
            .HasDatabaseName("IX_EmailInvites_ReferredEmailAddress");

        builder.HasIndex(e => e.UserEntityId)
            .HasDatabaseName("IX_EmailInvites_UserEntityId");

        builder.HasIndex(e => e.ExpireAt)
            .HasDatabaseName("IX_EmailInvites_ExpireAt");

        builder.HasIndex(e => e.IsSoftDeleted)
            .HasDatabaseName("IX_EmailInvites_IsSoftDeleted");
    }
}
