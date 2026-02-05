using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Services;

// Cloudflare R2 Object Storage
internal sealed class ObjectStorageService : IDisposable
{
    private readonly CloudflareR2Options _options;
    private readonly ILogger<ObjectStorageService> _logger;
    private readonly AmazonS3Client _s3Client;

    public ObjectStorageService(IOptions<CloudflareR2Options> options, ILogger<ObjectStorageService> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);
        
        _options = options.Value;
        _logger = logger;

        var credentials = new BasicAWSCredentials(_options.AccessKeyId, _options.SecretAccessKey);
        _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = _options.EndpointUrl, // R2-specific endpoint
            ForcePathStyle = true // Required for R2 compatibility
        });
    }

    public async Task DownloadObjectAsync(string bucketName, string filePath, string key)
    {
        ArgumentNullException.ThrowIfNull(bucketName);
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            using (GetObjectResponse? response = await _s3Client.GetObjectAsync(request))
            using (Stream? responseStream = response.ResponseStream)
            using (FileStream fileStream = File.Create(filePath))
            {
                await responseStream.CopyToAsync(fileStream);
            }

            _logger.LogInformation(ObjectStorageMessages.FileDownloadedSuccessfully, filePath);
        }
        catch (AmazonS3Exception e)
        {
            _logger.LogError(e, ObjectStorageMessages.ErrorReadingObject);
        }
    }

    public async Task UploadObjectAsync(string bucketName, string filePath, string key)
    {
        ArgumentNullException.ThrowIfNull(bucketName);
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            var request = new PutObjectRequest
            {
                FilePath = filePath,
#pragma warning disable CS0618 // Type or member is obsolete
                BucketName = _options.BucketName,
#pragma warning restore CS0618 // Type or member is obsolete
                Key = key
            };

            PutObjectResponse? response = await _s3Client.PutObjectAsync(request);
            _logger.LogInformation(ObjectStorageMessages.ETagFormat, response.ETag);
        }
        catch (AmazonS3Exception e)
        {
            _logger.LogError(e, ObjectStorageMessages.ErrorWritingObject);
        }
    }

    public void Dispose()
    {
        _s3Client?.Dispose();
    }
}
