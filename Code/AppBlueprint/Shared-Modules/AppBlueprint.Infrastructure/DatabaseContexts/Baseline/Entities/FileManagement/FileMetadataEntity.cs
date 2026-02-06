using System.Text.Json;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;

/// <summary>
/// Stores file metadata in PostgreSQL with custom fields as JSONB.
/// The actual file content is stored in Cloudflare R2.
/// </summary>
public sealed class FileMetadataEntity : BaseEntity, ITenantScoped
{
    public FileMetadataEntity()
    {
        Id = PrefixedUlid.Generate("file");
    }

    /// <summary>
    /// Unique storage key in R2 (e.g., "tenant_123/images/guid-filename.jpg").
    /// </summary>
    public required string FileKey { get; init; }

    /// <summary>
    /// Original filename provided by user.
    /// </summary>
    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public required string OriginalFileName { get; init; }

    /// <summary>
    /// MIME type (e.g., "image/jpeg", "application/pdf").
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long SizeInBytes { get; init; }

    /// <summary>
    /// User ID who uploaded the file.
    /// </summary>
    public required string UploadedBy { get; init; }

    /// <summary>
    /// Optional folder/category for organizing files.
    /// </summary>
    public string? Folder { get; init; }

    /// <summary>
    /// Whether the file is publicly accessible (for dating app images).
    /// If false, requires pre-signed URL for access.
    /// </summary>
    public bool IsPublic { get; init; }

    /// <summary>
    /// Public URL for files stored in public bucket (dating app images).
    /// Null for private files.
    /// </summary>
    public Uri? PublicUrl { get; init; }

    /// <summary>
    /// Custom metadata stored as JSONB in PostgreSQL.
    /// Example: image dimensions, property listing ID, document type, etc.
    /// </summary>
    public Dictionary<string, string>? CustomMetadata { get; init; }

    /// <summary>
    /// Tenant ID for multi-tenant isolation.
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Serializes custom metadata to JSON for logging or API responses.
    /// </summary>
    public string GetCustomMetadataJson()
    {
        return CustomMetadata is not null ? JsonSerializer.Serialize(CustomMetadata) : "{}";
    }
}
