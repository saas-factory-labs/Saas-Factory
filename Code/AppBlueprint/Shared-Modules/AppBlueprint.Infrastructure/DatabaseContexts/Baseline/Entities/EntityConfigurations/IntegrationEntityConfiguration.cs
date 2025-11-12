using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Integration.EntityConfigurations;

/// <summary>
/// Entity configuration for IntegrationEntity managing third-party service integrations.
/// Handles secure storage of API keys and provides efficient querying by service type and owner.
/// </summary>
public sealed class IntegrationEntityConfiguration : BaseEntityConfiguration<IntegrationEntity>
{
    public override void Configure(EntityTypeBuilder<IntegrationEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        // Apply base configuration including named soft delete filter
        base.Configure(builder);
        // Define table name
        builder.ToTable("Integrations");

        // Define primary key
        builder.HasKey(e => e.Id)
            .HasName("PK_Integrations");

        // Configure Id property
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Primary key for integration");

        // Configure Name property
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Friendly name for the integration");

        // Configure ServiceName property (Stripe, SendGrid, Twilio, etc.)
        builder.Property(e => e.ServiceName)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Name of the third-party service (e.g., Stripe, SendGrid, Twilio)");

        // Configure Description property
        builder.Property(e => e.Description)
            .IsRequired(false)
            .HasMaxLength(500)
            .HasComment("Optional description of the integration purpose");

        // Configure API key reference with GDPR compliance
        builder.Property(e => e.ApiKeySecretReference)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Reference to the securely stored API key")
            .HasAnnotation("DataClassification", GDPRType.SensitiveMiscellaneous);

        // Configure OwnerId property
        builder.Property(e => e.OwnerId)
            .IsRequired()
            .HasComment("Foreign key to the user who owns this integration");

        // Configure timestamp properties
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Timestamp when the integration was created");

        builder.Property(e => e.LastUpdatedAt)
            .IsRequired(false)
            .HasComment("Timestamp when the integration was last updated");

        // Configure relationship to Owner (User)
        builder.HasOne<UserEntity>() // UserEntity doesn't have Integrations navigation property
            .WithMany()
            .HasForeignKey(i => i.OwnerId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Integrations_Users_OwnerId");

        // Create indexes for performance
        builder.HasIndex(i => i.OwnerId)
            .HasDatabaseName("IX_Integrations_OwnerId")
            .HasFilter(null);

        builder.HasIndex(i => i.ServiceName)
            .HasDatabaseName("IX_Integrations_ServiceName")
            .HasFilter(null);

        builder.HasIndex(i => i.CreatedAt)
            .HasDatabaseName("IX_Integrations_CreatedAt")
            .HasFilter(null);

        // Create composite indexes for common queries
        builder.HasIndex(i => new { i.OwnerId, i.ServiceName })
            .IsUnique()
            .HasDatabaseName("IX_Integrations_OwnerId_ServiceName_Unique")
            .HasFilter(null);

        builder.HasIndex(i => new { i.ServiceName, i.CreatedAt })
            .HasDatabaseName("IX_Integrations_ServiceName_CreatedAt")
            .HasFilter(null);
    }
}
