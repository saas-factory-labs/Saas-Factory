using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using AppBlueprint.Application.Interfaces.Storage;
using AppBlueprint.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Storage.Services;

/// <summary>
/// Cloudflare R2 storage service implementation using AWS S3 SDK.
/// Based on official Cloudflare R2 documentation: https://developers.cloudflare.com/r2/examples/aws/aws-sdk-net/
/// </summary>
internal sealed class R2FileStorageService : IFileStorageService, IDisposable
{
    private readonly CloudflareR2Options _options;
    private readonly ILogger<R2FileStorageService> _logger;
    private readonly IAmazonS3 _s3Client;

    public R2FileStorageService(IOptions<CloudflareR2Options> options, ILogger<R2FileStorageService> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _options = options.Value;
        _logger = logger;

        var credentials = new BasicAWSCredentials(_options.AccessKeyId, _options.SecretAccessKey);
        _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = _options.EndpointUrl,
            Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds),
            MaxErrorRetry = 3,
            LogResponse = _options.EnableLogging,
            LogMetrics = _options.EnableLogging
        });

        if (_options.EnableLogging)
        {
            _logger.LogDebug("R2FileStorageService initialized with endpoint: {Endpoint}, bucket: {Bucket}",
                _options.EndpointUrl, _options.BucketName);
        }
    }

    /// <inheritdoc />
    public async Task<FileUploadResult> UploadAsync(
        Stream stream,
        string key,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(contentType);

        try
        {
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = contentType,
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true
            };

            PutObjectResponse response = await _s3Client.PutObjectAsync(request, cancellationToken);
            string url = GeneratePublicUrl(key);
            long size = stream.Length;

            _logger.LogInformation("File uploaded to R2: {Key}, ETag: {ETag}, Size: {Size} bytes",
                key, response.ETag, size);

            return new FileUploadResult(key, url, size, response.ETag, contentType);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to R2: {Key}, StatusCode: {StatusCode}",
                key, ex.StatusCode);
            throw new InvalidOperationException($"Failed to upload file to R2: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<FileUploadResult> UploadFileAsync(
        string localFilePath,
        string key,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(localFilePath);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(contentType);

        if (!File.Exists(localFilePath))
            throw new FileNotFoundException($"File not found: {localFilePath}");

        FileInfo fileInfo = new(localFilePath);
        if (fileInfo.Length > _options.MaxFileSizeBytes)
        {
            throw new InvalidOperationException(
                $"File size {fileInfo.Length} bytes exceeds maximum allowed size of {_options.MaxFileSizeBytes} bytes.");
        }

        try
        {
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                FilePath = localFilePath,
                ContentType = contentType,
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true
            };

            PutObjectResponse response = await _s3Client.PutObjectAsync(request, cancellationToken);
            string url = GeneratePublicUrl(key);

            _logger.LogInformation("File uploaded from disk to R2: {Key}, ETag: {ETag}, Size: {Size} bytes",
                key, response.ETag, fileInfo.Length);

            return new FileUploadResult(key, url, fileInfo.Length, response.ETag, contentType);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error uploading file from disk to R2: {Key}, StatusCode: {StatusCode}",
                key, ex.StatusCode);
            throw new InvalidOperationException($"Failed to upload file to R2: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<FileDownloadResult> DownloadAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            GetObjectResponse response = await _s3Client.GetObjectAsync(request, cancellationToken);

            _logger.LogInformation("File downloaded from R2: {Key}, Size: {Size} bytes",
                key, response.ContentLength);

            return new FileDownloadResult(
                response.ResponseStream,
                response.Headers.ContentType,
                response.ContentLength,
                response.ETag);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("File not found in R2: {Key}", key);
            throw new FileNotFoundException($"File not found in R2: {key}", ex);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error downloading file from R2: {Key}, StatusCode: {StatusCode}",
                key, ex.StatusCode);
            throw new InvalidOperationException($"Failed to download file from R2: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> DownloadFileAsync(
        string key,
        string localFilePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(localFilePath);

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            using GetObjectResponse response = await _s3Client.GetObjectAsync(request, cancellationToken);

            string? directory = Path.GetDirectoryName(localFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await response.WriteResponseStreamToFileAsync(localFilePath, append: false, cancellationToken);

            _logger.LogInformation("File downloaded from R2 to disk: {Key}, Path: {Path}",
                key, localFilePath);

            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("File not found in R2: {Key}", key);
            return false;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error downloading file from R2 to disk: {Key}, StatusCode: {StatusCode}",
                key, ex.StatusCode);
            throw new InvalidOperationException($"Failed to download file from R2: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);

            _logger.LogInformation("File deleted from R2: {Key}", key);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("File not found in R2 during delete: {Key}", key);
            return false;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from R2: {Key}, StatusCode: {StatusCode}",
                key, ex.StatusCode);
            throw new InvalidOperationException($"Failed to delete file from R2: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence in R2: {Key}, StatusCode: {StatusCode}",
                key, ex.StatusCode);
            throw new InvalidOperationException($"Failed to check file existence in R2: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public string GeneratePresignedUrl(string key, TimeSpan expiresIn)
    {
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            AWSConfigsS3.UseSignatureVersion4 = true;

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.Add(expiresIn)
            };

            string url = _s3Client.GetPreSignedURL(request);

            _logger.LogDebug("Generated presigned download URL for R2: {Key}, Expires in: {ExpiresIn}",
                key, expiresIn);

            return url;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for R2: {Key}", key);
            throw new InvalidOperationException($"Failed to generate presigned URL: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public string GeneratePresignedUploadUrl(string key, string contentType, TimeSpan expiresIn)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(contentType);

        try
        {
            AWSConfigsS3.UseSignatureVersion4 = true;

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.Add(expiresIn),
                ContentType = contentType
            };

            string url = _s3Client.GetPreSignedURL(request);

            _logger.LogDebug("Generated presigned upload URL for R2: {Key}, Expires in: {ExpiresIn}",
                key, expiresIn);

            return url;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned upload URL for R2: {Key}", key);
            throw new InvalidOperationException($"Failed to generate presigned upload URL: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> ListFilesAsync(
        string prefix,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _options.BucketName,
                Prefix = prefix,
                MaxKeys = 1000
            };

            var files = new List<string>();
            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request, cancellationToken);
                files.AddRange(response.S3Objects.Select(obj => obj.Key));
                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated);

            _logger.LogInformation("Listed {Count} files from R2 with prefix: {Prefix}", files.Count, prefix);

            return files.AsReadOnly();
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error listing files from R2 with prefix: {Prefix}, StatusCode: {StatusCode}",
                prefix, ex.StatusCode);
            throw new InvalidOperationException($"Failed to list files from R2: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<FileMetadata?> GetMetadataAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            GetObjectMetadataResponse response = await _s3Client.GetObjectMetadataAsync(request, cancellationToken);

            return new FileMetadata(
                key,
                response.ContentLength,
                response.ETag,
                response.LastModified,
                response.Headers.ContentType);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("File metadata not found in R2: {Key}", key);
            return null;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error getting file metadata from R2: {Key}, StatusCode: {StatusCode}",
                key, ex.StatusCode);
            throw new InvalidOperationException($"Failed to get file metadata from R2: {ex.Message}", ex);
        }
    }

    private string GeneratePublicUrl(string key)
    {
        string baseUrl = string.IsNullOrEmpty(_options.PublicUrlDomain)
            ? $"{_options.EndpointUrl}/{_options.BucketName}"
            : _options.PublicUrlDomain;

        return $"{baseUrl.TrimEnd('/')}/{key.TrimStart('/')}";
    }

    public void Dispose()
    {
        _s3Client?.Dispose();
    }
}
