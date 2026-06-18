# File Storage Implementation

Complete guide for the Cloudflare R2 file storage implementation supporting multiple use cases: dating app, property rental portal, and CRM applications.

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Setup Guide](#setup-guide)
- [Configuration](#configuration)
- [Usage Examples](#usage-examples)
- [API Endpoints](#api-endpoints)
- [Security](#security)
- [Demo Page](#demo-page)
- [Troubleshooting](#troubleshooting)

---

## Overview

### Features

✅ **Dual Bucket Architecture**
- **Private Bucket**: Secure files requiring authentication (CRM documents, rental agreements, contracts)
- **Public Bucket**: Publicly accessible files with GUID-based URL obscurity (dating app profile images)

✅ **Tenant Isolation**
- Automatic tenant scoping on all operations
- Multi-tenant data separation using JWT claims
- Query filters prevent cross-tenant data leaks

✅ **Flexible Metadata Storage**
- Custom metadata stored as JSONB in PostgreSQL
- No schema changes needed for new metadata fields
- Supports complex queries on custom fields

✅ **Security Features**
- File type validation with magic byte checking
- Size limits per file type (images: 10MB, documents: 50MB, videos: 500MB)
- Dangerous extension blacklist (.exe, .bat, .ps1, etc.)
- Pre-signed URLs with time-limited access (default: 1 hour)

✅ **Cloudflare R2 Optimized**
- AWS S3 SDK compatibility
- Follows official Cloudflare R2 .NET guidelines
- Global edge network for fast access
- Zero egress fees

---

## Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────┐
│  Presentation Layer (AppBlueprint.Web)              │
│  - FileStorageDemo.razor (demo UI)                  │
│  - FileStorageController.cs (REST API)              │
└──────────────────┬──────────────────────────────────┘
                   │ depends on ↓
┌──────────────────▼──────────────────────────────────┐
│  Application Layer (AppBlueprint.Application)       │
│  - IFileStorageService (contract)                   │
│  - IFileValidationService (contract)                │
│  - CloudflareR2Options (configuration)              │
└──────────────────┬──────────────────────────────────┘
                   │ depends on ↓
┌──────────────────▼──────────────────────────────────┐
│  Infrastructure Layer (AppBlueprint.Infrastructure) │
│  - R2FileStorageService (implementation)            │
│  - FileValidationService (implementation)           │
│  - FileMetadataEntity (database model)              │
│  - FileMetadataRepository (data access)             │
└─────────────────────────────────────────────────────┘
```

### Database Schema

**FileMetadata Table:**
```sql
CREATE TABLE "FileMetadata" (
    "Id" VARCHAR(1024) PRIMARY KEY,
    "FileKey" VARCHAR(500) UNIQUE NOT NULL,
    "OriginalFileName" VARCHAR(255) NOT NULL,
    "ContentType" VARCHAR(100) NOT NULL,
    "SizeInBytes" BIGINT NOT NULL,
    "UploadedBy" VARCHAR(50) NOT NULL,
    "Folder" VARCHAR(200),
    "IsPublic" BOOLEAN DEFAULT FALSE,
    "PublicUrl" VARCHAR(1000),
    "CustomMetadata" JSONB DEFAULT '{}',
    "TenantId" VARCHAR(50) NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    "LastUpdatedAt" TIMESTAMPTZ,
    "IsSoftDeleted" BOOLEAN DEFAULT FALSE
);

-- Indexes for performance
CREATE INDEX "IX_FileMetadata_TenantId" ON "FileMetadata" ("TenantId");
CREATE INDEX "IX_FileMetadata_FileKey" ON "FileMetadata" ("FileKey");
CREATE INDEX "IX_FileMetadata_TenantId_Folder" ON "FileMetadata" ("TenantId", "Folder");
CREATE INDEX "IX_FileMetadata_TenantId_UploadedBy" ON "FileMetadata" ("TenantId", "UploadedBy");
CREATE INDEX "IX_FileMetadata_CreatedAt" ON "FileMetadata" ("CreatedAt");
```

---

## Setup Guide

### Prerequisites

1. **Cloudflare Account** with R2 enabled
2. **.NET 10 SDK** installed
3. **PostgreSQL 14+** database
4. **Doppler** (for development) or environment variables (for production)

### Step 1: Get Cloudflare R2 Credentials

#### 1.1 Get Your Account ID

1. Log in to [Cloudflare Dashboard](https://dash.cloudflare.com/)
2. Your account ID is in the URL: `dash.cloudflare.com/[YOUR-ACCOUNT-ID]`
3. Or go to **R2** → **Overview** to see your account ID

#### 1.2 Create R2 API Token

1. Go to **R2** → **Manage R2 API Tokens**
2. Click **Create API Token**
3. Set permissions:
   - **Object Read & Write**: ✅ Enabled
   - **Admin Read & Write**: ❌ Disabled (not needed)
4. Optionally restrict to specific buckets
5. Click **Create API Token**
6. Copy the **Access Key ID** and **Secret Access Key**
   - ⚠️ **Save these immediately** - you won't see the secret again!

Example output:
```
Access Key ID: abc123def456ghi789jkl012
Secret Access Key: abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGH
```

#### 1.3 Create R2 Buckets

**Option A: Using Cloudflare Dashboard**

1. Go to **R2** → **Overview**
2. Click **Create bucket**
3. Create two buckets:
   - `appblueprint-private` - For secure files
   - `appblueprint-public` - For public files
4. Configure CORS for public bucket (if using custom domain):
   ```json
   {
     "AllowedOrigins": ["*"],
     "AllowedMethods": ["GET", "HEAD"],
     "AllowedHeaders": ["*"],
     "MaxAgeSeconds": 3600
   }
   ```

**Option B: Using Wrangler CLI**

```bash
# Install Wrangler
npm install -g wrangler

# Authenticate
wrangler login

# Create private bucket
wrangler r2 bucket create appblueprint-private

# Create public bucket
wrangler r2 bucket create appblueprint-public
```

#### 1.4 Build Endpoint URL

Format: `https://[YOUR-ACCOUNT-ID].r2.cloudflarestorage.com`

Example: `https://abc123def456.r2.cloudflarestorage.com`

#### 1.5 (Optional) Set Up Custom Domain for Public Bucket

1. Go to **R2** → **Your public bucket** → **Settings**
2. Scroll to **Custom Domains**
3. Click **Connect Domain**
4. Enter your domain (e.g., `cdn.yourdomain.com`)
5. Follow DNS configuration instructions
6. Wait for DNS propagation (up to 24 hours)

---

## Configuration

### Development (Doppler)

Set these variables in Doppler:

```bash
# Required
APPBLUEPRINT_CLOUDFLARE_R2_ACCESSKEYID=abc123def456ghi789jkl012
APPBLUEPRINT_CLOUDFLARE_R2_SECRETACCESSKEY=abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGH
APPBLUEPRINT_CLOUDFLARE_R2_ENDPOINTURL=https://abc123def456.r2.cloudflarestorage.com
APPBLUEPRINT_CLOUDFLARE_R2_PRIVATEBUCKETNAME=appblueprint-private
APPBLUEPRINT_CLOUDFLARE_R2_PUBLICBUCKETNAME=appblueprint-public

# Optional
APPBLUEPRINT_CLOUDFLARE_R2_PUBLICDOMAIN=https://cdn.yourdomain.com
```

**How to set in Doppler:**

```powershell
# Open Doppler secrets editor
doppler secrets

# Or set via CLI
doppler secrets set APPBLUEPRINT_CLOUDFLARE_R2_ACCESSKEYID="your-value"
doppler secrets set APPBLUEPRINT_CLOUDFLARE_R2_SECRETACCESSKEY="your-secret"
doppler secrets set APPBLUEPRINT_CLOUDFLARE_R2_ENDPOINTURL="https://abc123.r2.cloudflarestorage.com"
doppler secrets set APPBLUEPRINT_CLOUDFLARE_R2_PRIVATEBUCKETNAME="appblueprint-private"
doppler secrets set APPBLUEPRINT_CLOUDFLARE_R2_PUBLICBUCKETNAME="appblueprint-public"
```

### Production (Environment Variables)

Set without the `APPBLUEPRINT_` prefix:

```bash
# Required
CLOUDFLARE_R2_ACCESSKEYID=abc123def456ghi789jkl012
CLOUDFLARE_R2_SECRETACCESSKEY=abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGH
CLOUDFLARE_R2_ENDPOINTURL=https://abc123def456.r2.cloudflarestorage.com
CLOUDFLARE_R2_PRIVATEBUCKETNAME=appblueprint-private
CLOUDFLARE_R2_PUBLICBUCKETNAME=appblueprint-public

# Optional
CLOUDFLARE_R2_PUBLICDOMAIN=https://cdn.yourdomain.com
```

**Platform-Specific Instructions:**

**Railway.app:**
```bash
railway variables set CLOUDFLARE_R2_ACCESSKEYID=your-value
railway variables set CLOUDFLARE_R2_SECRETACCESSKEY=your-secret
# ... etc
```

**Azure App Service:**
```bash
az webapp config appsettings set \
  --name your-app-name \
  --resource-group your-rg \
  --settings \
    CLOUDFLARE_R2_ACCESSKEYID=your-value \
    CLOUDFLARE_R2_SECRETACCESSKEY=your-secret
```

**Docker / docker-compose.yml:**
```yaml
services:
  web:
    environment:
      - CLOUDFLARE_R2_ACCESSKEYID=your-value
      - CLOUDFLARE_R2_SECRETACCESSKEY=your-secret
      - CLOUDFLARE_R2_ENDPOINTURL=https://abc123.r2.cloudflarestorage.com
      - CLOUDFLARE_R2_PRIVATEBUCKETNAME=appblueprint-private
      - CLOUDFLARE_R2_PUBLICBUCKETNAME=appblueprint-public
```

### Alternative: appsettings.json (Not Recommended for Production)

```json
{
  "Cloudflare": {
    "R2": {
      "AccessKeyId": "your-access-key-id",
      "SecretAccessKey": "your-secret-access-key",
      "EndpointUrl": "https://abc123def456.r2.cloudflarestorage.com",
      "PrivateBucketName": "appblueprint-private",
      "PublicBucketName": "appblueprint-public",
      "PublicDomain": "https://cdn.yourdomain.com"
    }
  }
}
```

⚠️ **Security Warning**: Never commit credentials to source control! Use environment variables or secret management tools.

---

## Usage Examples

### C# Application Code

#### Upload a File

```csharp
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Services;

public class MyService
{
    private readonly IFileStorageService _fileStorage;

    public MyService(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<string> UploadProfileImageAsync(Stream imageStream, string fileName)
    {
        var request = new UploadFileRequest
        {
            FileName = fileName,
            FileStream = imageStream,
            ContentType = "image/jpeg",
            Folder = "profile-images",
            IsPublic = true, // Public for dating app
            CustomMetadata = new Dictionary<string, string>
            {
                { "user_type", "premium" },
                { "image_category", "profile" }
            }
        };

        StoredFile result = await _fileStorage.UploadAsync(request);
        return result.PublicUrl; // Direct URL
    }

    public async Task<string> UploadContractAsync(Stream contractStream, string fileName)
    {
        var request = new UploadFileRequest
        {
            FileName = fileName,
            FileStream = contractStream,
            ContentType = "application/pdf",
            Folder = "contracts",
            IsPublic = false, // Private for CRM
            CustomMetadata = new Dictionary<string, string>
            {
                { "contract_type", "rental_agreement" },
                { "expires_at", "2026-12-31" }
            }
        };

        StoredFile result = await _fileStorage.UploadAsync(request);
        
        // Generate pre-signed URL for private access
        string presignedUrl = await _fileStorage.GetPreSignedUrlAsync(
            result.FileKey, 
            TimeSpan.FromHours(2)
        );
        
        return presignedUrl;
    }
}
```

#### Download a File

```csharp
public async Task<byte[]> DownloadFileAsync(string fileKey)
{
    FileDownloadResult download = await _fileStorage.DownloadAsync(fileKey);
    
    using var memoryStream = new MemoryStream();
    await download.FileStream.CopyToAsync(memoryStream);
    return memoryStream.ToArray();
}
```

#### List Files

```csharp
public async Task<List<StoredFile>> GetUserFilesAsync(string folder)
{
    var query = new FileListQuery
    {
        Folder = folder,
        Limit = 50,
        SortByCreatedAt = true
    };

    return await _fileStorage.ListFilesAsync(query);
}
```

#### Delete Files

```csharp
// Delete single file
await _fileStorage.DeleteAsync(fileKey);

// Delete multiple files
await _fileStorage.DeleteManyAsync(new[] { fileKey1, fileKey2, fileKey3 });
```

---

## API Endpoints

All endpoints require authentication and are automatically tenant-scoped.

### POST `/api/v1/filestorage/upload`

Upload a file with multipart/form-data.

**Request:**
```bash
curl -X POST https://your-api.com/api/v1/filestorage/upload \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "file=@profile.jpg" \
  -F "folder=profile-images" \
  -F "isPublic=true" \
  -F 'customMetadata={"user_role":"admin"}'
```

**Response:**
```json
{
  "fileKey": "profile-images/01JBCDEF123456789ABCDEFGHI.jpg",
  "originalFileName": "profile.jpg",
  "contentType": "image/jpeg",
  "sizeInBytes": 1048576,
  "uploadedBy": "usr_abc123",
  "uploadedAt": "2026-02-02T12:34:56Z",
  "folder": "profile-images",
  "isPublic": true,
  "publicUrl": "https://abc123.r2.cloudflarestorage.com/appblueprint-public/profile-images/01JBCDEF123456789ABCDEFGHI.jpg",
  "customMetadata": {
    "user_role": "admin"
  }
}
```

### GET `/api/v1/filestorage/download/{fileKey}`

Download a file (streams file content).

**Request:**
```bash
curl -X GET https://your-api.com/api/v1/filestorage/download/contracts%2F01JBC.pdf \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -o downloaded-file.pdf
```

### POST `/api/v1/filestorage/presigned-url`

Generate a pre-signed URL for temporary private file access.

**Request:**
```json
{
  "fileKey": "contracts/01JBCDEF123456789ABCDEFGHI.pdf",
  "expiryMinutes": 120
}
```

**Response:**
```json
{
  "url": "https://abc123.r2.cloudflarestorage.com/appblueprint-private/contracts/01JBC.pdf?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=...",
  "expiresAt": "2026-02-02T14:34:56Z"
}
```

### GET `/api/v1/filestorage/list`

List files with optional filtering.

**Query Parameters:**
- `folder` (optional): Filter by folder
- `limit` (optional): Max results (default: 100)

**Response:**
```json
[
  {
    "fileKey": "profile-images/01JBCDEF.jpg",
    "originalFileName": "avatar.jpg",
    "contentType": "image/jpeg",
    "sizeInBytes": 524288,
    "folder": "profile-images",
    "isPublic": true,
    "publicUrl": "https://...",
    "customMetadata": { "category": "avatar" }
  }
]
```

### DELETE `/api/v1/filestorage/{fileKey}`

Delete a single file.

### POST `/api/v1/filestorage/delete-many`

Delete multiple files in one operation.

**Request:**
```json
{
  "fileKeys": [
    "profile-images/01JBCDEF.jpg",
    "documents/01JBCXYZ.pdf"
  ]
}
```

---

## Security

### File Validation

**Allowed File Types:**
- **Images**: `image/jpeg`, `image/png`, `image/gif`, `image/webp`, `image/svg+xml`
- **Documents**: `application/pdf`, `application/msword`, `application/vnd.openxmlformats-officedocument.*`, `text/plain`, `text/csv`
- **Videos**: `video/mp4`, `video/webm`, `video/quicktime`

**Size Limits:**
- Images: 10 MB
- Documents: 50 MB
- Videos: 500 MB

**Security Checks:**
1. ✅ Content type validation against whitelist
2. ✅ File size enforcement
3. ✅ Dangerous extension blacklist (`.exe`, `.bat`, `.cmd`, `.ps1`, `.sh`, `.scr`, `.vbs`)
4. ✅ Magic byte validation for images (prevents spoofing)
5. ✅ Filename sanitization (removes path traversal attempts)

### Tenant Isolation

**Defense-in-Depth Strategy:**

1. **JWT Claims Extraction**: Tenant ID from authenticated user token
2. **Service-Level Validation**: Every operation validates tenant ownership
3. **Repository Query Filters**: EF Core query filters automatically scope queries
4. **Database Row-Level Security**: PostgreSQL RLS policies (if enabled)

Example tenant check:
```csharp
// Automatic tenant validation on every operation
var file = await _dbContext.FileMetadata
    .Where(f => f.FileKey == fileKey)
    // Query filter automatically adds: && f.TenantId == currentTenantId
    .FirstOrDefaultAsync();

if (file == null)
    throw new UnauthorizedAccessException("File not found or access denied");
```

### Pre-Signed URLs

- Time-limited access (default: 1 hour, configurable)
- No authentication required once URL is generated
- Expire automatically after timeout
- Cannot be extended (must generate new URL)

---

## Demo Page

Test the file storage system at: `/filestorage-demo`

### Features:

1. **Upload Files**
   - Select file from disk
   - Specify folder (optional)
   - Toggle public/private access
   - Add custom metadata as JSON

2. **View Files**
   - See all your uploaded files
   - View metadata and custom fields
   - See public/private status

3. **Manage Files**
   - Generate pre-signed URLs for private files
   - Copy public URLs for public files
   - Download files
   - Delete files

4. **Real-Time Updates**
   - Refresh file list after operations
   - See file sizes and upload timestamps

**Access:** Navigate to `https://your-app.com/filestorage-demo` (requires authentication)

---

## Troubleshooting

### Issue: "Unable to load credentials"

**Cause:** R2 credentials not configured correctly.

**Solution:**
1. Verify environment variables are set correctly
2. Check variable names are UPPERCASE
3. Confirm Doppler is running: `doppler run dotnet watch`
4. Check logs for specific missing variables

### Issue: "Access Denied" when uploading

**Cause:** R2 API token lacks permissions or bucket doesn't exist.

**Solution:**
1. Verify API token has **Object Read & Write** permissions
2. Confirm buckets exist: `wrangler r2 bucket list`
3. Check bucket names match environment variables exactly

### Issue: "File validation failed"

**Cause:** File type, size, or content doesn't meet requirements.

**Solution:**
1. Check file type is in allowed list (see [Security](#security))
2. Verify file size is under limit
3. For images, ensure file isn't corrupted (magic byte check)

### Issue: Pre-signed URLs not working

**Cause:** Bucket is not configured correctly or URL expired.

**Solution:**
1. Verify `EndpointUrl` is correct
2. Check bucket names match configuration
3. Confirm URL hasn't expired (default: 1 hour)
4. Generate a new pre-signed URL

### Issue: Public URLs return 403 Forbidden

**Cause:** Public bucket CORS or access settings misconfigured.

**Solution:**
1. Verify using `PublicBucketName`, not `PrivateBucketName`
2. Check bucket CORS settings in Cloudflare Dashboard
3. Ensure custom domain (if used) is properly configured

### Issue: Files from other tenants are visible

**Cause:** Tenant isolation not working correctly.

**Solution:**
1. Verify JWT token contains tenant claim
2. Check `ITenantContextAccessor` is resolving tenant correctly
3. Review query filters are applied
4. Check database migrations are applied

---

## Database Migration

Apply the file storage migration:

```powershell
# Navigate to Infrastructure project
cd Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure

# Apply migration
dotnet ef database update --context BaselineDbContext
```

Migration creates:
- `FileMetadata` table with JSONB support
- Indexes for tenant-scoped queries
- Unique constraint on `FileKey`

---

## Performance Considerations

### Cloudflare R2 Benefits

1. **Global Edge Network**: Files served from nearest datacenter
2. **Zero Egress Fees**: No charges for data transfer out
3. **S3 Compatibility**: Easy migration from AWS S3
4. **Automatic Compression**: Cloudflare handles compression

### Optimization Tips

1. **Use Custom Domain**: Enable CDN caching for public files
2. **Batch Operations**: Use `DeleteManyAsync` for bulk deletes
3. **Limit List Queries**: Set reasonable `Limit` values
4. **Index Custom Metadata**: Create GIN indexes on JSONB fields if querying frequently

```sql
-- Example: Index for custom metadata queries
CREATE INDEX idx_file_metadata_custom 
ON "FileMetadata" USING GIN ("CustomMetadata");
```

---

## Cost Estimation

### Cloudflare R2 Pricing (as of 2026)

- **Storage**: $0.015/GB/month
- **Class A Operations** (write, list): $4.50/million requests
- **Class B Operations** (read): $0.36/million requests
- **Egress**: **$0.00** (FREE!)

### Example Monthly Cost

**Scenario**: Dating app with 10,000 users, 5 images each

- Storage: 500 MB avg/user × 10,000 = 5 TB = $75/month
- Uploads: 50,000 images = 50,000 Class A ops = $0.23
- Views: 1M views/month = 1M Class B ops = $0.36
- **Total: ~$75.59/month**

Compare to AWS S3: Storage ($115) + Egress ($90 for 1TB) = **$205/month**

**Savings with R2: 63% cheaper than AWS S3**

---

## Support

For issues or questions:
- Check [Troubleshooting](#troubleshooting)
- Review [Cloudflare R2 Documentation](https://developers.cloudflare.com/r2/)
- Check application logs for detailed error messages
- Contact your DevOps team for infrastructure issues

---

## Changelog

### v1.0.0 (2026-02-02)
- ✅ Initial implementation with Cloudflare R2
- ✅ Dual bucket architecture (public/private)
- ✅ Tenant isolation with JWT claims
- ✅ JSONB custom metadata support
- ✅ File validation with magic byte checking
- ✅ Pre-signed URLs for secure access
- ✅ Demo page at `/filestorage-demo`
- ✅ Database migration `20260202085124_AddFileMetadataEntity`

---

**Last Updated**: February 2, 2026
