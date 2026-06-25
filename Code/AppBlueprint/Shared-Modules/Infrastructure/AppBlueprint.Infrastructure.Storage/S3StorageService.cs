using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using AppBlueprint.Application.Exceptions;
using AppBlueprint.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AppBlueprint.Infrastructure.Storage;

/// <summary>
/// Secure S3-compatible document storage implementation.
/// Designed for Cloudflare R2 via custom ServiceURL; also works with AWS S3, MinIO, or any S3-compatible provider.
/// </summary>
public sealed class S3StorageService : IStorageService, IDisposable
{
    // %PDF — first 4 bytes of any valid PDF (ISO 32000-1)
    private static readonly byte[] PdfMagicBytes = [0x25, 0x50, 0x44, 0x46];

    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly ILogger<S3StorageService> _logger;

    public S3StorageService(IConfiguration configuration, ILogger<S3StorageService> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;

        string endpoint = configuration["STORAGE_ENDPOINT"]
            ?? throw new InvalidOperationException("STORAGE_ENDPOINT is not configured");
        string accessKey = configuration["STORAGE_ACCESS_KEY"]
            ?? throw new InvalidOperationException("STORAGE_ACCESS_KEY is not configured");
        string secretKey = configuration["STORAGE_SECRET_KEY"]
            ?? throw new InvalidOperationException("STORAGE_SECRET_KEY is not configured");
        _bucketName = configuration["STORAGE_BUCKET_NAME"]
            ?? throw new InvalidOperationException("STORAGE_BUCKET_NAME is not configured");

        _s3Client = new AmazonS3Client(
            new BasicAWSCredentials(accessKey, secretKey),
            new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true  // Required for R2 and most S3-compatible providers
            });
    }

    public async Task<string> UploadDocumentAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        if (!fileStream.CanSeek)
            throw new ArgumentException("File stream must support seeking for magic bytes validation.", nameof(fileStream));

        await ValidatePdfMagicBytesAsync(fileStream, cancellationToken);

        string sanitizedName = SanitizeFileName(fileName);
        // Prefix with a UUID path segment to guarantee uniqueness and prevent enumeration
        string objectKey = $"documents/{Guid.NewGuid():N}/{sanitizedName}";

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            InputStream = fileStream,
            ContentType = "application/pdf",
            AutoCloseStream = false,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true
        };

        // Forces browser to download rather than render/execute inline.
        // Mitigates stored-XSS and script-injection in embedded PDFs.
        request.Headers["Content-Disposition"] = $"attachment; filename=\"{sanitizedName}\"";

        // Security intent metadata.
        // A CDN layer (e.g. Cloudflare Transform Rules) should promote these to actual HTTP response headers.
        // R2 stores them as x-amz-meta-* and returns them on GET — wire up Transform Rules to re-emit them.
        request.Metadata["x-amz-meta-x-content-type-options"] = "nosniff";
        request.Metadata["x-amz-meta-content-security-policy"] = "default-src 'none';";

        try
        {
            PutObjectResponse response = await _s3Client.PutObjectAsync(request, cancellationToken);
            _logger.LogInformation(
                "Document uploaded. Key: {ObjectKey}, ETag: {ETag}",
                objectKey, response.ETag);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 upload failed. Key: {ObjectKey}", objectKey);
            throw new InvalidOperationException($"Failed to upload document: {ex.Message}", ex);
        }

        return objectKey;
    }

    /// <summary>
    /// Reads the first 4 bytes of the stream and compares against the PDF magic byte sequence (%PDF).
    /// Resets the stream position to 0 after reading regardless of outcome.
    /// Does NOT rely on file extension or client-supplied Content-Type.
    /// </summary>
    private static async Task ValidatePdfMagicBytesAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (stream.Length < PdfMagicBytes.Length)
            throw new InvalidFileTypeException(
                "File is too small to be a valid PDF document.");

        var header = new byte[PdfMagicBytes.Length];
        int bytesRead = await stream.ReadAsync(header.AsMemory(0, PdfMagicBytes.Length), cancellationToken);

        // Always reset before returning — upload must start from position 0
        stream.Seek(0, SeekOrigin.Begin);

        if (bytesRead < PdfMagicBytes.Length)
            throw new InvalidFileTypeException(
                "Unable to read file header for type validation.");

        if (!header.AsSpan().SequenceEqual(PdfMagicBytes))
            throw new InvalidFileTypeException(
                "File signature does not match PDF format. " +
                "Expected magic bytes: 0x25 0x50 0x44 0x46 (%PDF).");
    }

    /// <summary>
    /// Strips all path components and allows only [a-z0-9], dash, and underscore.
    /// Enforces a .pdf extension regardless of the original name.
    /// Maximum base name length of 100 characters to stay within S3 key limits.
    /// </summary>
    private static string SanitizeFileName(string fileName)
    {
        // Path.GetFileName neutralises directory traversal (../../, \\..\\ etc.)
        string baseName = Path.GetFileNameWithoutExtension(Path.GetFileName(fileName));

        var safe = new StringBuilder(Math.Min(baseName.Length, 100));
        foreach (char c in baseName)
        {
            if (char.IsLetterOrDigit(c) || c is '-' or '_')
                safe.Append(char.ToLowerInvariant(c));
            else if (c is ' ')
                safe.Append('-');
            // All other characters (dots, slashes, null bytes, etc.) are silently dropped
        }

        string result = safe.Length > 0 ? safe.ToString() : "document";

        if (result.Length > 100)
            result = result[..100];

        return result + ".pdf";
    }

    public void Dispose() => _s3Client.Dispose();
}
