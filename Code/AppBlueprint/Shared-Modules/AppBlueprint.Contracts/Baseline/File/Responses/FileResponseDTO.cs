namespace AppBlueprint.Contracts.Baseline.File.Responses;

/// <summary>
/// Response containing file metadata after upload or retrieval.
/// </summary>
public sealed class FileStorageResponse
{
    /// <summary>
    /// Unique file key in storage.
    /// </summary>
    public required string FileKey { get; init; }

    /// <summary>
    /// Original file name.
    /// </summary>
    public required string OriginalFileName { get; init; }

    /// <summary>
    /// MIME type.
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
    /// Upload timestamp.
    /// </summary>
    public DateTime UploadedAt { get; init; }

    /// <summary>
    /// Optional folder/category.
    /// </summary>
    public string? Folder { get; init; }

    /// <summary>
    /// Whether the file is publicly accessible.
    /// </summary>
    public bool IsPublic { get; init; }

    /// <summary>
    /// Public URL (only for public files).
    /// </summary>
    public Uri? PublicUrl { get; init; }

    /// <summary>
    /// Custom metadata.
    /// </summary>
    public Dictionary<string, string>? CustomMetadata { get; init; }
}

/// <summary>
/// Response containing a pre-signed URL for secure file access.
/// </summary>
public sealed class PreSignedUrlResponse
{
    /// <summary>
    /// Pre-signed URL with time-limited access.
    /// </summary>
    public required Uri Url { get; init; }

    /// <summary>
    /// URL expiration timestamp.
    /// </summary>
    public DateTime ExpiresAt { get; init; }
}

// Keep old response for backward compatibility
public class FileResponse
{
    public string? FileName { get; init; }
}
