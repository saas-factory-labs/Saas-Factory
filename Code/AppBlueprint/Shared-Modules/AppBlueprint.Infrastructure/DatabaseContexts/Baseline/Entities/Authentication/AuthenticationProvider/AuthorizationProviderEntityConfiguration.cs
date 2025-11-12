using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;

/// <summary>
/// Entity configuration for AuthenticationProviderEntity managing external authentication providers.
/// Configures authentication providers like Google OAuth, Microsoft Azure AD, Auth0, etc.
/// </summary>
public sealed class AuthorizationProviderEntityConfiguration : BaseEntityConfiguration<AuthenticationProviderEntity>
{
    /// <summary>
    /// Configures the AuthenticationProviderEntity with table mapping, properties, and indexes.
    /// </summary>
    /// <param name="builder">The entity type builder for AuthenticationProviderEntity</param>
    /// <exception cref="ArgumentNullException">Thrown when builder is null</exception>
    public override void Configure(EntityTypeBuilder<AuthenticationProviderEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
// Table configuration
        builder.ToTable("AuthenticationProviders");
        builder.HasKey(e => e.Id);

        // Primary key configuration
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Unique identifier for the authentication provider");

        // Provider name (required, unique)
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Name of the authentication provider (e.g., Google, Microsoft, Auth0, GitHub)");

        // Active status flag
        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Indicates if this authentication provider is currently active and available for use");

        // Audit fields
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Timestamp when the authentication provider was created");

        builder.Property(e => e.LastUpdatedAt)
            .IsRequired(false)
            .HasComment("Timestamp when the authentication provider was last updated");

        // Indexes for performance
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_AuthenticationProviders_Name");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("IX_AuthenticationProviders_IsActive");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_AuthenticationProviders_CreatedAt");

        // Note: AuthenticationProviderEntity serves as a reference table for external auth providers.
        // Relationships to users or auth sessions would be configured in their respective entity configurations.
    }
}
