using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;
using AppBlueprint.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Cloudflare R2 file storage implementation with tenant isolation.
/// Supports both public (dating app images) and private (CRM documents) file access patterns.
/// </summary>
public sealed class R2FileStorageService : IFileStorageService, IDisposable
{
    private readonly CloudflareR2Options _options;
    private readonly ILogger<R2FileStorageService> _logger;
    private readonly AmazonS3Client _s3Client;
    private readonly BaselineDbContext _dbContext;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public R2FileStorageService(
        IOptions<CloudflareR2Options> options,
        ILogger<R2FileStorageService> logger,
        BaselineDbContext dbContext,
        ITenantContextAccessor tenantContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(tenantContextAccessor);

        _options = options.Value;
        _logger = logger;
        _dbContext = dbContext;
        _tenantContextAccessor = tenantContextAccessor;

        var credentials = new BasicAWSCredentials(_options.AccessKeyId, _options.SecretAccessKey);
        _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = _options.EndpointUrl,
            ForcePathStyle = true,
            Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds)
        });
    }

    public async Task<StoredFile> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.FileStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.FileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ContentType);

        string tenantId = _tenantContextAccessor.TenantId ?? throw new InvalidOperationException("Tenant context not available");
        string userId = GetCurrentUserId();
        
        // Generate unique file key: tenant_xxx/folder/guid-filename
        string fileKey = GenerateFileKey(tenantId, request.Folder, request.FileName);
        string bucketName = request.IsPublic ? _options.PublicBucketName : _options.PrivateBucketName;

        try
        {
            // Upload to R2
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileKey,
                InputStream = request.FileStream,
                ContentType = request.ContentType,
                AutoCloseStream = false,
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true
            };

            // Add custom metadata as S3 object metadata (not stored in DB)
            if (request.CustomMetadata is not null)
            {
                foreach (var (key, value) in request.CustomMetadata)
                {
                    putRequest.Metadata.Add($"x-amz-meta-{key}", value);
                }
            }

            PutObjectResponse? response = await _s3Client.PutObjectAsync(putRequest, cancellationToken);
            _logger.LogInformation("File uploaded to R2: {FileKey}, ETag: {ETag}", fileKey, response.ETag);

            // Get file size
            long fileSize = request.FileStream.Length;

            // Generate public URL if applicable (null baseUrl will use R2 direct URL or custom domain)
            Uri? publicUrl = request.IsPublic ? GetPublicUrl(fileKey, baseUrl: null) : null;

            // Save metadata to PostgreSQL with JSONB custom fields
            var metadata = new FileMetadataEntity
            {
                FileKey = fileKey,
                OriginalFileName = request.FileName,
                ContentType = request.ContentType,
                SizeInBytes = fileSize,
                UploadedBy = userId,
                Folder = request.Folder,
                IsPublic = request.IsPublic,
                PublicUrl = publicUrl,
                CustomMetadata = request.CustomMetadata,
                TenantId = tenantId
            };

            await _dbContext.Set<FileMetadataEntity>().AddAsync(metadata, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("File metadata saved to database: {FileKey}, Size: {Size} bytes", fileKey, fileSize);

            return new StoredFile(
                fileKey,
                request.FileName,
                request.ContentType,
                fileSize,
                tenantId,
                userId,
                metadata.CreatedAt,
                request.Folder,
                request.IsPublic,
                publicUrl,
                request.CustomMetadata
            );
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file to R2: {FileKey}", fileKey);
            throw new InvalidOperationException($"Failed to upload file: {ex.Message}", ex);
        }
    }

    public async Task<FileDownloadResult> DownloadAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileKey);

        string tenantId = _tenantContextAccessor.TenantId ?? throw new InvalidOperationException("Tenant context not available");

        // Verify tenant ownership
        FileMetadataEntity? metadata = await _dbContext.Set<FileMetadataEntity>()
            .FirstOrDefaultAsync(f => f.FileKey == fileKey && f.TenantId == tenantId, cancellationToken);

        if (metadata is null)
        {
            _logger.LogWarning("File not found or access denied: {FileKey}, Tenant: {TenantId}", fileKey, tenantId);
            throw new UnauthorizedAccessException($"File not found or access denied: {fileKey}");
        }

        string bucketName = metadata.IsPublic ? _options.PublicBucketName : _options.PrivateBucketName;

        try
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileKey
            };

            GetObjectResponse? response = await _s3Client.GetObjectAsync(getRequest, cancellationToken);
            
            _logger.LogInformation("File downloaded from R2: {FileKey}", fileKey);

            return new FileDownloadResult(
                response.ResponseStream,
                metadata.ContentType,
                metadata.OriginalFileName,
                metadata.SizeInBytes
            );
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to download file from R2: {FileKey}", fileKey);
            throw new InvalidOperationException($"Failed to download file: {ex.Message}", ex);
        }
    }

    public async Task<FileDownloadResult> DownloadPublicAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileKey);

        // Verify file is public (requires tenant context via RLS)
        FileMetadataEntity? metadata = await _dbContext.Set<FileMetadataEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.FileKey == fileKey && f.IsPublic == true, cancellationToken);

        if (metadata is null)
        {
            _logger.LogWarning("Public file not found: {FileKey}", fileKey);
            throw new UnauthorizedAccessException($"Public file not found: {fileKey}");
        }

        try
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = _options.PublicBucketName,
                Key = fileKey
            };

            GetObjectResponse? response = await _s3Client.GetObjectAsync(getRequest, cancellationToken);
            
            _logger.LogInformation("Public file downloaded from R2: {FileKey}", fileKey);

            return new FileDownloadResult(
                response.ResponseStream,
                metadata.ContentType,
                metadata.OriginalFileName,
                metadata.SizeInBytes
            );
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to download public file from R2: {FileKey}", fileKey);
            throw new InvalidOperationException($"Failed to download public file: {ex.Message}", ex);
        }
    }

    public async Task<string> GetPreSignedUrlAsync(string fileKey, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileKey);

        string tenantId = _tenantContextAccessor.TenantId ?? throw new InvalidOperationException("Tenant context not available");

        // Verify tenant ownership
        bool exists = await _dbContext.Set<FileMetadataEntity>()
            .AnyAsync(f => f.FileKey == fileKey && f.TenantId == tenantId, cancellationToken);

        if (!exists)
        {
            _logger.LogWarning("File not found or access denied for pre-signed URL: {FileKey}, Tenant: {TenantId}", fileKey, tenantId);
            throw new UnauthorizedAccessException($"File not found or access denied: {fileKey}");
        }

        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.PrivateBucketName,
                Key = fileKey,
                Expires = DateTime.UtcNow.Add(expiry),
                Verb = HttpVerb.GET
            };

            string url = await _s3Client.GetPreSignedURLAsync(request);
            
            _logger.LogInformation("Pre-signed URL generated for file: {FileKey}, Expiry: {Expiry}", fileKey, expiry);

            return url;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to generate pre-signed URL: {FileKey}", fileKey);
            throw new InvalidOperationException($"Failed to generate pre-signed URL: {ex.Message}", ex);
        }
    }

    public Uri GetPublicUrl(string fileKey, Uri? baseUrl = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileKey);

        // Use API endpoint if provided (for authentication-free downloads through the API)
        if (baseUrl is not null)
        {
            _logger.LogInformation("Using API endpoint for public URL: {BaseUrl}", baseUrl);
            string urlString = $"{baseUrl.ToString().TrimEnd('/')}/api/v1/filestorage/public/{Uri.EscapeDataString(fileKey)}";
            return new Uri(urlString, UriKind.Absolute);
        }

        // Use custom domain if configured, otherwise use R2 endpoint
        if (!string.IsNullOrWhiteSpace(_options.PublicDomain))
        {
            _logger.LogInformation("Using PublicDomain for public URL: {PublicDomain}", _options.PublicDomain);
            string urlString = $"{_options.PublicDomain.TrimEnd('/')}/{fileKey}";
            return new Uri(urlString, UriKind.Absolute);
        }

        // R2 public bucket URL format (requires bucket to be publicly accessible)
        _logger.LogWarning("PublicDomain not configured, falling back to R2 endpoint URL (requires auth): {EndpointUrl}", _options.EndpointUrl);
        string fallbackUrl = $"{_options.EndpointUrl.TrimEnd('/')}/{_options.PublicBucketName}/{fileKey}";
        return new Uri(fallbackUrl, UriKind.Absolute);
    }

    public async Task<IEnumerable<StoredFile>> ListFilesAsync(FileListQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        string tenantId = _tenantContextAccessor.TenantId ?? throw new InvalidOperationException("Tenant context not available");

        IQueryable<FileMetadataEntity> queryable = _dbContext.Set<FileMetadataEntity>()
            .Where(f => f.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(query.Folder))
        {
            queryable = queryable.Where(f => f.Folder == query.Folder);
        }

        if (!string.IsNullOrWhiteSpace(query.FileNamePrefix))
        {
            queryable = queryable.Where(f => f.OriginalFileName.StartsWith(query.FileNamePrefix));
        }

        if (query.MaxResults.HasValue)
        {
            queryable = queryable.Take(query.MaxResults.Value);
        }

        List<FileMetadataEntity>? results = await queryable
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);

        return results.Select(m => new StoredFile(
            m.FileKey,
            m.OriginalFileName,
            m.ContentType,
            m.SizeInBytes,
            m.TenantId,
            m.UploadedBy,
            m.CreatedAt,
            m.Folder,
            m.IsPublic,
            m.PublicUrl,
            m.CustomMetadata
        ));
    }

    public async Task DeleteAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileKey);

        string? tenantId = _tenantContextAccessor.TenantId;
        
        _logger.LogInformation("DeleteAsync called - FileKey: {FileKey}, TenantId: {TenantId}, HasTenantContext: {HasContext}", 
            fileKey, tenantId ?? "NULL", tenantId is not null);

        if (tenantId is null)
        {
            _logger.LogError("Tenant context not available for delete operation. FileKey: {FileKey}", fileKey);
            throw new InvalidOperationException("Tenant context not available");
        }
        
        _logger.LogInformation("Proceeding with delete - TenantId: {TenantId}", tenantId);

        // Verify tenant ownership
        FileMetadataEntity? metadata = await _dbContext.Set<FileMetadataEntity>()
            .FirstOrDefaultAsync(f => f.FileKey == fileKey && f.TenantId == tenantId, cancellationToken);
        
        _logger.LogInformation("Metadata query result - Found: {Found}, FileKey: {FileKey}", metadata is not null, fileKey);

        if (metadata is null)
        {
            _logger.LogWarning("File not found or access denied for deletion: {FileKey}, Tenant: {TenantId}", fileKey, tenantId);
            throw new UnauthorizedAccessException($"File not found or access denied: {fileKey}");
        }

        string bucketName = metadata.IsPublic ? _options.PublicBucketName : _options.PrivateBucketName;

        try
        {
            // Delete from R2
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = fileKey
            };

            await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
            _logger.LogInformation("File deleted from R2: {FileKey}", fileKey);

            // Delete metadata from database
            _dbContext.Set<FileMetadataEntity>().Remove(metadata);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("File metadata deleted from database: {FileKey}", fileKey);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file from R2: {FileKey}", fileKey);
            throw new InvalidOperationException($"Failed to delete file: {ex.Message}", ex);
        }
    }

    public async Task DeleteManyAsync(IEnumerable<string> fileKeys, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileKeys);

        string tenantId = _tenantContextAccessor.TenantId ?? throw new InvalidOperationException("Tenant context not available");
        List<string>? fileKeysList = fileKeys.ToList();

        if (fileKeysList.Count == 0)
        {
            return;
        }

        // Verify tenant ownership for all files
        List<FileMetadataEntity>? metadataList = await _dbContext.Set<FileMetadataEntity>()
            .Where(f => fileKeysList.Contains(f.FileKey) && f.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        if (metadataList.Count != fileKeysList.Count)
        {
            _logger.LogWarning("Some files not found or access denied for bulk deletion, Tenant: {TenantId}", tenantId);
            throw new UnauthorizedAccessException("Some files not found or access denied");
        }

        try
        {
            // Group files by bucket for efficient deletion
            var publicFiles = metadataList.Where(m => m.IsPublic).Select(m => m.FileKey).ToList();
            var privateFiles = metadataList.Where(m => !m.IsPublic).Select(m => m.FileKey).ToList();

            // Delete public files
            if (publicFiles.Count > 0)
            {
                await DeleteMultipleFromBucketAsync(_options.PublicBucketName, publicFiles, cancellationToken);
            }

            // Delete private files
            if (privateFiles.Count > 0)
            {
                await DeleteMultipleFromBucketAsync(_options.PrivateBucketName, privateFiles, cancellationToken);
            }

            // Delete metadata from database
            _dbContext.Set<FileMetadataEntity>().RemoveRange(metadataList);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Bulk deleted {Count} files", fileKeysList.Count);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to bulk delete files from R2");
            throw new InvalidOperationException($"Failed to bulk delete files: {ex.Message}", ex);
        }
    }

    private async Task DeleteMultipleFromBucketAsync(string bucketName, List<string> fileKeys, CancellationToken cancellationToken)
    {
        var deleteRequest = new DeleteObjectsRequest
        {
            BucketName = bucketName,
            Objects = fileKeys.Select(k => new KeyVersion { Key = k }).ToList()
        };

        await _s3Client.DeleteObjectsAsync(deleteRequest, cancellationToken);
    }

    private static string GenerateFileKey(string tenantId, string? folder, string fileName)
    {
        // Generate unique GUID-based key for obscurity (dating app pattern)
        string uniqueId = Guid.NewGuid().ToString("N");
        string sanitizedFileName = SanitizeFileName(fileName);
        
        if (!string.IsNullOrWhiteSpace(folder))
        {
            return $"{tenantId}/{folder}/{uniqueId}-{sanitizedFileName}";
        }

        return $"{tenantId}/{uniqueId}-{sanitizedFileName}";
    }

    private static string SanitizeFileName(string fileName)
    {
        // Remove invalid characters and keep extension
        string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        string sanitized = string.Concat(fileName.Split(invalidChars.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrWhiteSpace(sanitized) ? "file" : sanitized;
    }

    private string GetCurrentUserId()
    {
        // TODO: Replace with actual user context from authentication
        // For now, return a placeholder - this should be injected via IHttpContextAccessor or similar
        return "usr_placeholder";
    }

    public void Dispose()
    {
        _s3Client?.Dispose();
    }
}
