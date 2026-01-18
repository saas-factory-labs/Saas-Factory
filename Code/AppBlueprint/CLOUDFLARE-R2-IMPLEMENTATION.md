# Cloudflare R2 Storage Implementation

Complete, production-ready Cloudflare R2 storage service implementation using the AWS SDK for .NET, following official Cloudflare R2 documentation and clean architecture principles.

## 📋 Overview

This implementation provides:
- ✅ **Official Cloudflare R2 patterns** - Based on [Cloudflare R2 AWS SDK .NET docs](https://developers.cloudflare.com/r2/examples/aws/aws-sdk-net/)
- ✅ **Clean Architecture** - Interface in Application layer, implementation in Infrastructure
- ✅ **Complete file operations** - Upload, download, delete, list, exists, metadata
- ✅ **Presigned URLs** - For direct client uploads and secure downloads
- ✅ **Stream & file support** - Upload from Stream or file system
- ✅ **Error handling** - Proper exception handling with meaningful error messages
- ✅ **Structured logging** - Comprehensive logging for debugging and monitoring
- ✅ **Type-safe configuration** - Validated options with data annotations
- ✅ **Database integration** - FileController integrates metadata tracking with R2 storage

## 🏗️ Architecture

```
┌─────────────────────────────────────────────┐
│  Presentation Layer (FileController)        │
│  - Upload/Download endpoints                │
│  - Presigned URL generation                 │
│  - Database metadata tracking               │
└──────────────┬──────────────────────────────┘
               │ depends on ↓
┌──────────────▼──────────────────────────────┐
│  Application Layer                          │
│  - IFileStorageService interface            │
│  - FileUploadResult, FileDownloadResult     │
│  - FileMetadata models                      │
└──────────────┬──────────────────────────────┘
               │ implemented by ↓
┌──────────────▼──────────────────────────────┐
│  Infrastructure Layer                       │
│  - CloudflareR2Service implementation       │
│  - AWS S3 SDK integration                   │
│  - CloudflareR2Options configuration        │
└─────────────────────────────────────────────┘
```

## 📦 Files Created/Modified

### Application Layer
- ✅ `IFileStorageService.cs` - Storage abstraction interface
- ✅ `CloudflareR2Options.cs` - Configuration options (enhanced)

### Infrastructure Layer
- ✅ `CloudflareR2Service.cs` - R2 implementation (replaced ObjectStorageService)
- ✅ `ServiceCollectionExtensions.cs` - DI registration (updated)

### Presentation Layer
- ✅ `FileController.cs` - Enhanced with R2 integration
- ✅ `FileUploadResponse.cs` - Response DTOs

### Configuration
- ✅ `appsettings.cloudflare-r2.example.json` - Example configuration

## ⚙️ Configuration

### 1. Get Cloudflare R2 Credentials

1. Go to [Cloudflare Dashboard](https://dash.cloudflare.com/)
2. Navigate to **R2** → **Overview**
3. Click **Manage R2 API Tokens**
4. Create a new API token:
   - **Permissions**: Read & Write
   - **Bucket**: Select your bucket or "All buckets"
5. Copy the **Access Key ID**, **Secret Access Key**, and **Endpoint URL**

### 2. Configure Application

Add to `appsettings.json` or environment variables:

```json
{
  "Cloudflare": {
    "R2": {
      "AccessKeyId": "your-access-key-id",
      "SecretAccessKey": "your-secret-access-key",
      "EndpointUrl": "https://<your-account-id>.r2.cloudflarestorage.com",
      "AccountId": "your-account-id",
      "BucketName": "appblueprint-files",
      "PublicUrlDomain": "https://files.yourapp.com",
      "TimeoutSeconds": 30,
      "MaxFileSizeBytes": 104857600,
      "EnableLogging": false
    }
  }
}
```

### Environment Variables (Recommended for Production)

```bash
export APPBLUEPRINT_Cloudflare__R2__AccessKeyId="your-access-key-id"
export APPBLUEPRINT_Cloudflare__R2__SecretAccessKey="your-secret-access-key"
export APPBLUEPRINT_Cloudflare__R2__EndpointUrl="https://<account-id>.r2.cloudflarestorage.com"
export APPBLUEPRINT_Cloudflare__R2__AccountId="your-account-id"
export APPBLUEPRINT_Cloudflare__R2__BucketName="appblueprint-files"
```

### Railway/Cloud Deployment

Use Railway's environment variables:
```
APPBLUEPRINT_Cloudflare__R2__AccessKeyId
APPBLUEPRINT_Cloudflare__R2__SecretAccessKey
APPBLUEPRINT_Cloudflare__R2__EndpointUrl
APPBLUEPRINT_Cloudflare__R2__BucketName
```

## 🚀 Usage

### 1. Upload File from Form

```csharp
// POST /api/v1/file/upload?folder=avatars
// Form-data: file = [binary]

var response = await httpClient.PostAsync(
    "/api/v1/file/upload?folder=avatars",
    multipartContent);

// Response:
{
  "id": "file_01HXY...",
  "fileName": "profile.jpg",
  "url": "https://files.yourapp.com/avatars/file_01HXY...jpg",
  "size": 52048,
  "contentType": "image/jpeg",
  "uploadedAt": "2026-01-17T10:30:00Z"
}
```

### 2. Generate Presigned Upload URL (Client-Side Upload)

```csharp
// POST /api/v1/file/presigned-upload-url?fileName=document.pdf&contentType=application/pdf&folder=documents&expiresInMinutes=15

var response = await httpClient.PostAsync("/api/v1/file/presigned-upload-url?...", null);

// Response:
{
  "uploadUrl": "https://<account-id>.r2.cloudflarestorage.com/...",
  "key": "documents/file_01HXY...pdf",
  "fileId": "file_01HXY...",
  "expiresAt": "2026-01-17T10:45:00Z"
}

// Client-side: Direct upload to R2
await fetch(uploadUrl, {
  method: 'PUT',
  headers: { 'Content-Type': 'application/pdf' },
  body: fileBlob
});
```

### 3. Download File

```csharp
// GET /api/v1/file/download/file_01HXY...

var stream = await httpClient.GetStreamAsync("/api/v1/file/download/file_01HXY...");
```

### 4. Generate Presigned Download URL

```csharp
// GET /api/v1/file/file_01HXY.../presigned-url?expiresInHours=1

var response = await httpClient.GetAsync("/api/v1/file/file_01HXY.../presigned-url?expiresInHours=1");

// Response:
{
  "url": "https://<account-id>.r2.cloudflarestorage.com/...",
  "expiresAt": "2026-01-17T11:30:00Z"
}
```

### 5. Programmatic Usage (Service Injection)

```csharp
public class AvatarService
{
    private readonly IFileStorageService _storage;

    public AvatarService(IFileStorageService storage)
    {
        _storage = storage;
    }

    public async Task<string> UploadAvatarAsync(Stream imageStream, string userId)
    {
        string key = $"avatars/{userId}.jpg";
        
        FileUploadResult result = await _storage.UploadAsync(
            imageStream,
            key,
            "image/jpeg");

        return result.Url;
    }

    public async Task<bool> DeleteAvatarAsync(string userId)
    {
        string key = $"avatars/{userId}.jpg";
        return await _storage.DeleteAsync(key);
    }
}
```

## 🔐 Security Features

### 1. Presigned URLs (Temporary Access)
- Generate time-limited URLs for client-side uploads/downloads
- No need to expose R2 credentials to clients
- URLs expire automatically

### 2. File Size Limits
- Default: 100MB (`MaxFileSizeBytes`)
- Configurable per deployment
- Cloudflare R2 limit: 5GB per file

### 3. Content-Type Validation
- Enforced at upload time
- Prevents MIME type mismatches

### 4. Tenant Isolation
- File keys can include tenant IDs: `{tenantId}/avatars/{fileId}.jpg`
- Database metadata tracks ownership

## 📊 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/v1/file/upload` | Upload file from form |
| `GET` | `/api/v1/file/download/{id}` | Download file by ID |
| `GET` | `/api/v1/file/{id}/presigned-url` | Get presigned download URL |
| `POST` | `/api/v1/file/presigned-upload-url` | Get presigned upload URL |
| `DELETE` | `/api/v1/file/DeleteFile/{id}` | Delete file and metadata |

## 🧪 Testing

### Manual Testing with cURL

#### Upload File
```bash
curl -X POST "https://localhost:7071/api/v1/file/upload?folder=avatars" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "file=@profile.jpg"
```

#### Download File
```bash
curl -X GET "https://localhost:7071/api/v1/file/download/file_01HXY..." \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  --output downloaded-file.jpg
```

#### Generate Presigned Upload URL
```bash
curl -X POST "https://localhost:7071/api/v1/file/presigned-upload-url?fileName=document.pdf&contentType=application/pdf" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Unit Tests (TODO)

```csharp
[Test]
public async Task UploadAsync_ValidFile_ReturnsUploadResult()
{
    // Arrange
    var mockOptions = new CloudflareR2Options { /* ... */ };
    var service = new CloudflareR2Service(Options.Create(mockOptions), Mock.Of<ILogger>());
    
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
    
    // Act
    FileUploadResult result = await service.UploadAsync(
        stream,
        "test/file.txt",
        "text/plain");
    
    // Assert
    result.Should().NotBeNull();
    result.Key.Should().Be("test/file.txt");
    result.Size.Should().BeGreaterThan(0);
}
```

## 🐛 Troubleshooting

### Issue: "AWSConfigsS3 does not exist"
**Solution**: Add `using Amazon;` to the using statements.

### Issue: "Payload signing error"
**Solution**: Ensure `DisablePayloadSigning = true` and `DisableDefaultChecksumValidation = true` are set (required for R2).

### Issue: "Endpoint URL invalid"
**Solution**: Verify endpoint format: `https://<account-id>.r2.cloudflarestorage.com`

### Issue: "Bucket not found"
**Solution**: 
1. Verify bucket exists in Cloudflare R2 dashboard
2. Check API token has access to the bucket
3. Ensure `BucketName` matches exactly (case-sensitive)

### Issue: "Service not registered"
**Solution**: Verify all configuration values are set (non-empty). Check console logs for:
```
[AppBlueprint.Infrastructure] Cloudflare R2 storage service registered
```

## 📚 Resources

- [Cloudflare R2 AWS SDK .NET Documentation](https://developers.cloudflare.com/r2/examples/aws/aws-sdk-net/)
- [Cloudflare R2 Overview](https://developers.cloudflare.com/r2/)
- [AWS SDK for .NET - S3 Client](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/S3/NS3.html)

## 🎯 Next Steps

1. ✅ **Complete**: Core R2 integration with official patterns
2. ⏳ **TODO**: Unit tests for CloudflareR2Service
3. ⏳ **TODO**: Integration tests with Testcontainers (MinIO)
4. ⏳ **TODO**: Image optimization pipeline (resize, compress, format conversion)
5. ⏳ **TODO**: Virus scanning integration (ClamAV)
6. ⏳ **TODO**: CDN integration (Cloudflare CDN for public files)
7. ⏳ **TODO**: Multipart upload for files >100MB

## 📝 Commit Message

```
feat: implement cloudflare r2 storage with aws sdk

- Add IFileStorageService interface in Application layer
- Implement CloudflareR2Service following official Cloudflare docs
- Enhance CloudflareR2Options with validation and new properties
- Update FileController with upload/download/presigned URL endpoints
- Add FileUploadResponse, PresignedUrlResponse DTOs
- Update DI registration for IFileStorageService
- Include appsettings.cloudflare-r2.example.json
- Add comprehensive README with setup and usage guide

Key features:
- Stream and file system upload support
- Presigned URLs for client-side uploads
- Download with stream or file system
- File metadata retrieval
- List files with prefix filtering
- Proper error handling and structured logging

Based on: https://developers.cloudflare.com/r2/examples/aws/aws-sdk-net/

Resolves: File storage infrastructure for avatars, documents, and attachments
```

---

## ✨ Summary

You now have a **production-ready Cloudflare R2 storage implementation** that:
- ✅ Follows official Cloudflare R2 patterns
- ✅ Uses clean architecture principles
- ✅ Supports all common file operations
- ✅ Integrates with database metadata
- ✅ Provides secure presigned URLs
- ✅ Has comprehensive error handling
- ✅ Is fully configurable and testable

Ready to deploy! 🚀
