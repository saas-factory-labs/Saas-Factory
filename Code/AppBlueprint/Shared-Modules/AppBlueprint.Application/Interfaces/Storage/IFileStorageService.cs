namespace AppBlueprint.Application.Interfaces.Storage;

/// <summary>
/// Abstraction for cloud file storage operations (Cloudflare R2, AWS S3, Azure Blob, etc.).
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file from a stream to cloud storage.
    /// </summary>
    /// <param name="stream">File content stream.</param>
    /// <param name="key">Storage key/path (e.g., "avatars/user_123.jpg").</param>
    /// <param name="contentType">MIME type (e.g., "image/jpeg").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload result with URL and metadata.</returns>
    Task<FileUploadResult> UploadAsync(
        Stream stream,
        string key,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a file from the local file system to cloud storage.
    /// </summary>
    /// <param name="localFilePath">Local file path.</param>
    /// <param name="key">Storage key/path (e.g., "documents/file_456.pdf").</param>
    /// <param name="contentType">MIME type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Upload result with URL and metadata.</returns>
    Task<FileUploadResult> UploadFileAsync(
        string localFilePath,
        string key,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from cloud storage to a stream.
    /// </summary>
    /// <param name="key">Storage key/path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Download result with stream and metadata.</returns>
    Task<FileDownloadResult> DownloadAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from cloud storage to the local file system.
    /// </summary>
    /// <param name="key">Storage key/path.</param>
    /// <param name="localFilePath">Destination local file path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful.</returns>
    Task<bool> DownloadFileAsync(
        string key,
        string localFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from cloud storage.
    /// </summary>
    /// <param name="key">Storage key/path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully.</returns>
    Task<bool> DeleteAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a file exists in cloud storage.
    /// </summary>
    /// <param name="key">Storage key/path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if file exists.</returns>
    Task<bool> ExistsAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a presigned URL for temporary access to a file (download).
    /// </summary>
    /// <param name="key">Storage key/path.</param>
    /// <param name="expiresIn">URL validity duration.</param>
    /// <returns>Presigned URL.</returns>
    string GeneratePresignedUrl(
        string key,
        TimeSpan expiresIn);

    /// <summary>
    /// Generates a presigned URL for uploading a file directly from the client.
    /// </summary>
    /// <param name="key">Storage key/path.</param>
    /// <param name="contentType">MIME type.</param>
    /// <param name="expiresIn">URL validity duration.</param>
    /// <returns>Presigned upload URL.</returns>
    string GeneratePresignedUploadUrl(
        string key,
        string contentType,
        TimeSpan expiresIn);

    /// <summary>
    /// Lists files with a given prefix.
    /// </summary>
    /// <param name="prefix">Key prefix (e.g., "avatars/").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of file keys.</returns>
    Task<IReadOnlyList<string>> ListFilesAsync(
        string prefix,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets file metadata without downloading the content.
    /// </summary>
    /// <param name="key">Storage key/path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File metadata.</returns>
    Task<FileMetadata?> GetMetadataAsync(
        string key,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a file upload operation.
/// </summary>
public sealed record FileUploadResult(
    string Key,
    string Url,
    long Size,
    string ETag,
    string ContentType);

/// <summary>
/// Result of a file download operation.
/// </summary>
public sealed record FileDownloadResult(
    Stream Stream,
    string ContentType,
    long ContentLength,
    string ETag) : IDisposable
{
    public void Dispose()
    {
        Stream?.Dispose();
    }
}

/// <summary>
/// File metadata information.
/// </summary>
public sealed record FileMetadata(
    string Key,
    long Size,
    string ETag,
    DateTime LastModified,
    string ContentType);
