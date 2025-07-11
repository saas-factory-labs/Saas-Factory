using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using AppBlueprint.Infrastructure.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Services;

// Cloudflare R2 Object Storage
internal sealed class ObjectStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ObjectStorageService> _logger;

    private readonly AmazonS3Client _s3Client;

    private ObjectStorageService(IConfiguration configuration, ILogger<ObjectStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        AccessKeyId = _configuration["ObjectStorage:AccessKeyId"] ?? throw new InvalidOperationException("ObjectStorage:AccessKeyId is not configured.");
        _SecretAccessKey = _configuration["ObjectStorage:SecretAccessKey"] ?? throw new InvalidOperationException("ObjectStorage:SecretAccessKey is not configured.");
        EndpointUrl = _configuration["ObjectStorage:EndpointUrl"] ?? throw new InvalidOperationException("ObjectStorage:EndpointUrl is not configured.");

        var credentials = new BasicAWSCredentials(AccessKeyId, _SecretAccessKey);
        _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = EndpointUrl, // R2-specific endpoint
            ForcePathStyle = true // Required for R2 compatibility
        });
    }

    public string AccessKeyId { get; set; }
    public string _SecretAccessKey { get; set; }
    public string EndpointUrl { get; set; }
    public string BucketName { get; set; }

    public async Task DownloadObjectAsync(string bucketName, string filePath, string key)
    {
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
            _logger.LogError(ObjectStorageMessages.ErrorReadingObject, e.Message);
        }
    }

    public async Task UploadObjectAsync(string bucketName, string filePath, string key)
    {
        try
        {
            var request = new PutObjectRequest
            {
                FilePath = filePath,
                BucketName = BucketName,
                Key = key
            };

            PutObjectResponse? response = await _s3Client.PutObjectAsync(request);
            _logger.LogInformation(ObjectStorageMessages.ETagFormat, response.ETag);
        }
        catch (AmazonS3Exception e)
        {
            _logger.LogError(ObjectStorageMessages.ErrorWritingObject, e.Message);
        }
    }
}
