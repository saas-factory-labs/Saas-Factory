using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Infrastructure.Services;

// Cloudflare R2 Object Storage
internal class ObjectStorageService
{
    private readonly IConfiguration _configuration;

    private readonly AmazonS3Client _s3Client;

    private ObjectStorageService(IConfiguration configuration)
    {
        _configuration = configuration;

        AccessKeyId = _configuration["ObjectStorage:AccessKeyId"];
        _SecretAccessKey = _configuration["ObjectStorage:SecretAccessKey"];
        EndpointUrl = _configuration["ObjectStorage:EndpointUrl"];

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

            Console.WriteLine("File downloaded successfully: {0}", filePath);
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine("Error encountered on server. Message: '{0}' when reading an object", e.Message);
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
            Console.WriteLine("ETag: {0}", response.ETag);
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
        }
    }
}
