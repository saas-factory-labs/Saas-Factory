using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Storage;

public sealed class ObjectStorageService(
    IOptions<ObjectStorageOptions> options,
    ILogger<ObjectStorageService> logger) : IObjectStorageService, IDisposable
{
    private readonly ObjectStorageOptions _options = options.Value;
    private readonly AmazonS3Client _s3Client = new(
        new BasicAWSCredentials(options.Value.AccessKey, options.Value.SecretKey),
        new AmazonS3Config
        {
            ServiceURL = options.Value.Endpoint,
            ForcePathStyle = true
        });

    public async Task UploadAsync(string objectKey, Stream fileStream, string contentType, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(objectKey);
        ArgumentNullException.ThrowIfNull(fileStream);
        ArgumentException.ThrowIfNullOrEmpty(contentType);

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            InputStream = fileStream,
            ContentType = contentType,
            AutoCloseStream = false,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true
        };

        try
        {
            PutObjectResponse response = await _s3Client.PutObjectAsync(request, cancellationToken);
            logger.LogInformation("Object uploaded. Key: {Key}, ETag: {ETag}", objectKey, response.ETag);
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogError(ex, "Failed to upload object. Key: {Key}", objectKey);
            throw new InvalidOperationException($"Failed to upload object '{objectKey}': {ex.Message}", ex);
        }
    }

    public async Task<Stream> DownloadAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(objectKey);

        var request = new GetObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey
        };

        try
        {
            GetObjectResponse response = await _s3Client.GetObjectAsync(request, cancellationToken);
            logger.LogInformation("Object downloaded. Key: {Key}", objectKey);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogError(ex, "Failed to download object. Key: {Key}", objectKey);
            throw new InvalidOperationException($"Failed to download object '{objectKey}': {ex.Message}", ex);
        }
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(objectKey);

        var request = new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey
        };

        try
        {
            await _s3Client.DeleteObjectAsync(request, cancellationToken);
            logger.LogInformation("Object deleted. Key: {Key}", objectKey);
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogError(ex, "Failed to delete object. Key: {Key}", objectKey);
            throw new InvalidOperationException($"Failed to delete object '{objectKey}': {ex.Message}", ex);
        }
    }

    public void Dispose() => _s3Client.Dispose();
}
