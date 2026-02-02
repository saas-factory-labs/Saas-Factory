namespace AppBlueprint.Contracts.Baseline.File.Requests;

/// <summary>
/// Request to upload a file with optional metadata.
/// </summary>
public sealed class UploadFileRequestDto
{
    /// <summary>
    /// Original file name.
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// MIME type of the file.
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// Optional folder/category for organizing files.
    /// </summary>
    public string? Folder { get; set; }

    /// <summary>
    /// Whether the file should be publicly accessible (for dating app images).
    /// If false, requires pre-signed URL for access (CRM documents, rental agreements).
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Custom metadata as key-value pairs (stored in JSONB).
    /// Example: { "property_id": "prop_123", "listing_type": "rental" }
    /// </summary>
    public Dictionary<string, string>? CustomMetadata { get; set; }
}

/// <summary>
/// Request to generate a pre-signed URL for secure file access.
/// </summary>
public sealed class GetPreSignedUrlRequest
{
    /// <summary>
    /// Unique file key in storage.
    /// </summary>
    public required string FileKey { get; set; }

    /// <summary>
    /// URL expiration time in seconds (default: 3600 = 1 hour).
    /// </summary>
    public int ExpirySeconds { get; set; } = 3600;
}

/// <summary>
/// Request to list files with optional filters.
/// </summary>
public sealed class ListFilesRequest
{
    /// <summary>
    /// Filter by folder.
    /// </summary>
    public string? Folder { get; set; }

    /// <summary>
    /// Filter by file name prefix.
    /// </summary>
    public string? FileNamePrefix { get; set; }

    /// <summary>
    /// Maximum number of results to return.
    /// </summary>
    public int? MaxResults { get; set; }
}
