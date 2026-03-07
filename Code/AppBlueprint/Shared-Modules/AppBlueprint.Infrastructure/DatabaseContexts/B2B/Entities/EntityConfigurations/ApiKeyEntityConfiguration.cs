using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for ApiKeyEntity defining database schema, relationships and constraints.
/// </summary>
public sealed class ApiKeyEntityConfiguration : IEntityTypeConfiguration<ApiKeyEntity>
{
    public void Configure(EntityTypeBuilder<ApiKeyEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table mapping with standardized naming
        builder.ToTable("ApiKeys");

        // Primary key
        builder.HasKey(t => t.Id)
            .HasName("PK_ApiKeys");

        // Properties with validation
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("The friendly name of the API key");

        builder.Property(t => t.Description)
            .HasMaxLength(500)
            .HasComment("Optional description of the API key purpose");

        builder.Property(t => t.SecretRef)
            .IsRequired()
            .HasMaxLength(255)
            .HasComment("Reference to the secret stored in Azure Key Vault")
            .HasAnnotation("SensitiveData", true);

        builder.Property(t => t.UserId)
            .IsRequired()
            .HasComment("Foreign key to the user who owns this API key");

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasComment("Timestamp when the API key was created");

        // Relationships
        builder.HasOne(t => t.Owner)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ApiKeys_Users_UserId");

        // Performance indexes with standardized naming
        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("IX_ApiKeys_UserId");

        builder.HasIndex(t => t.Name)
            .HasDatabaseName("IX_ApiKeys_Name");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_ApiKeys_CreatedAt");

        builder.Property(t => t.KeyHash)
            .HasMaxLength(64)
            .HasComment("SHA-256 hex hash of the raw API key for fast lookup");

        builder.Property(t => t.ExpiresAt)
            .HasComment("Optional expiry timestamp; null means the key never expires");

        builder.Property(t => t.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Whether this API key has been revoked");

        // Unique index enables O(1) lookup during authentication
        builder.HasIndex(t => t.KeyHash)
            .IsUnique()
            .HasDatabaseName("IX_ApiKeys_KeyHash");
    }
}
