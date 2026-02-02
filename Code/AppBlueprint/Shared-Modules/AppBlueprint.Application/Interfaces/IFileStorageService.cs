namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Service for managing file storage operations with Cloudflare R2.
/// Supports both public and private file access patterns with tenant isolation.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file to R2 storage with automatic tenant scoping.
    /// </summary>
    /// <param name="request">Upload request containing file stream and metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Metadata about the uploaded file including storage key and URL.</returns>
    Task<StoredFile> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Downloads a file from R2 storage with tenant validation.
    /// </summary>
    /// <param name="fileKey">Unique file key in storage.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File stream and metadata.</returns>
    Task<FileDownloadResult> DownloadAsync(string fileKey, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Downloads a public file from R2 storage without authentication or tenant validation.
    /// Only works for files marked as IsPublic=true.
    /// </summary>
    /// <param name="fileKey">Unique file key in storage.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File stream and metadata.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if file is not public.</exception>
    Task<FileDownloadResult> DownloadPublicAsync(string fileKey, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates a pre-signed URL for secure direct file access.
    /// Used for private files (CRM documents, rental agreements) that require time-limited access.
    /// </summary>
    /// <param name="fileKey">Unique file key in storage.</param>
    /// <param name="expiry">URL expiration time (default: 1 hour).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Time-limited pre-signed URL.</returns>
    Task<string> GetPreSignedUrlAsync(string fileKey, TimeSpan expiry, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates a public URL for files that don't require authentication.
    /// Used for dating app profile images with GUID-based obscurity.
    /// </summary>
    /// <param name="fileKey">Unique file key in storage.</param>
    /// <param name="baseUrl">Optional base URL for API endpoint. If null, uses R2 direct URL (requires custom domain).</param>
    /// <returns>Public URL to the file.</returns>
    string GetPublicUrl(string fileKey, string? baseUrl = null);
    
    /// <summary>
    /// Lists files for the current tenant with optional filtering.
    /// </summary>
    /// <param name="query">Query parameters for filtering.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of file metadata.</returns>
    Task<IEnumerable<StoredFile>> ListFilesAsync(FileListQuery query, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a file from R2 storage with tenant validation.
    /// </summary>
    /// <param name="fileKey">Unique file key in storage.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(string fileKey, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes multiple files in a single operation with tenant validation.
    /// </summary>
    /// <param name="fileKeys">List of file keys to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteManyAsync(IEnumerable<string> fileKeys, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a file stored in R2 with metadata.
/// </summary>
public sealed record StoredFile(
    string FileKey,
    string OriginalFileName,
    string ContentType,
    long SizeInBytes,
    string TenantId,
    string UploadedBy,
    DateTime UploadedAt,
    string? Folder,
    bool IsPublic,
    string? PublicUrl,
    Dictionary<string, string>? CustomMetadata
);

/// <summary>
/// Request for uploading a file to storage.
/// </summary>
public sealed record UploadFileRequest(
    Stream FileStream,
    string FileName,
    string ContentType,
    string? Folder = null,
    bool IsPublic = false,
    Dictionary<string, string>? CustomMetadata = null
);

/// <summary>
/// Result of a file download operation.
/// </summary>
public sealed record FileDownloadResult(
    Stream FileStream,
    string ContentType,
    string FileName,
    long SizeInBytes
);

/// <summary>
/// Query parameters for listing files.
/// </summary>
public sealed record FileListQuery(
    string? Folder = null,
    string? FileNamePrefix = null,
    int? MaxResults = null
);
