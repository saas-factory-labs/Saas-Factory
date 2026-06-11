using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Auditing.ApiLog;

/// <summary>
/// Entity Framework configuration for the ApiLogEntity.
/// This configuration defines the database mapping, constraints, and relationships for API request/response logging.
/// </summary>
public sealed class ApiLogEntityConfiguration : IEntityTypeConfiguration<ApiLogEntity>
{
    /// <summary>
    /// Configures the entity mapping for ApiLogEntity including table structure, property constraints, 
    /// relationships, and indexes for optimal API logging and audit trail functionality.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the ApiLogEntity.</param>
    /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
    public void Configure(EntityTypeBuilder<ApiLogEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Configure table name
        builder.ToTable("ApiLogs");

        // Configure primary key
        builder.HasKey(e => e.Id);

        // Configure properties with comprehensive validation
        builder.Property(e => e.Id)
            .IsRequired()
            .HasComment("Primary key for the API log entry");

        builder.Property(e => e.ApiKeyId)
            .IsRequired()
            .HasMaxLength(450) // Standard for foreign key fields
            .HasComment("Foreign key reference to the API key used for the request");

        builder.Property(e => e.SessionId)
            .IsRequired()
            .HasMaxLength(450) // Standard for session identifiers
            .HasComment("Session identifier for tracking user sessions");

        builder.Property(e => e.RequestPath)
            .IsRequired()
            .HasMaxLength(2000) // Allow for long URLs with query parameters
            .HasComment("The API endpoint path that was requested");

        builder.Property(e => e.StatusCode)
            .IsRequired()
            .HasComment("HTTP status code returned by the API");

        builder.Property(e => e.StatusMessage)
            .IsRequired()
            .HasMaxLength(500) // Allow for detailed error messages
            .HasComment("HTTP status message or custom response message");

        builder.Property(e => e.RequestMethod)
            .IsRequired()
            .HasMaxLength(10) // GET, POST, PUT, PATCH, DELETE, OPTIONS, HEAD
            .HasComment("HTTP method used for the request (GET, POST, PUT, PATCH, DELETE, etc.)");

        builder.Property(e => e.SourceIp)
            .IsRequired()
            .HasMaxLength(45) // IPv6 addresses can be up to 45 characters
            .HasComment("Source IP address of the client making the request");

        builder.Property(e => e.ResponseLatency)
            .IsRequired()
            .HasComment("Response time in milliseconds for performance monitoring");

        // Configure navigation properties
        builder.HasOne(e => e.SessionEntity)
            .WithMany()
            .HasForeignKey(e => e.SessionId)
            .HasPrincipalKey(s => s.SessionKey)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_ApiLogs_Sessions_SessionId");

        // Configure indexes for performance optimization
        builder.HasIndex(e => e.ApiKeyId)
            .HasDatabaseName("IX_ApiLogs_ApiKeyId")
            .HasFilter(null);

        builder.HasIndex(e => e.SessionId)
            .HasDatabaseName("IX_ApiLogs_SessionId")
            .HasFilter(null);

        builder.HasIndex(e => e.StatusCode)
            .HasDatabaseName("IX_ApiLogs_StatusCode")
            .HasFilter(null);

        builder.HasIndex(e => e.RequestMethod)
            .HasDatabaseName("IX_ApiLogs_RequestMethod")
            .HasFilter(null);

        builder.HasIndex(e => e.SourceIp)
            .HasDatabaseName("IX_ApiLogs_SourceIp")
            .HasFilter(null);

        // Composite index for common query patterns
        builder.HasIndex(e => new { e.ApiKeyId, e.StatusCode })
            .HasDatabaseName("IX_ApiLogs_ApiKeyId_StatusCode")
            .HasFilter(null);

        builder.HasIndex(e => new { e.SessionId, e.StatusCode })
            .HasDatabaseName("IX_ApiLogs_SessionId_StatusCode")
            .HasFilter(null);
    }
}
