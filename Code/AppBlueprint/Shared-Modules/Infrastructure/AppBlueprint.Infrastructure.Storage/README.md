# AppBlueprint.Infrastructure.Storage

S3-compatible object storage for AppBlueprint. Three layered abstractions cover every use case from raw byte I/O to secure, validated document delivery.

---

## Abstractions at a glance

| Interface | Implementation | Purpose |
|---|---|---|
| `IFileStorageService` | `R2FileStorageService` | Tenant-scoped file management — upload, download, list, delete, pre-signed URLs. Persists metadata to PostgreSQL. |
| `IObjectStorageService` | `ObjectStorageService` | Generic S3 primitive — stream upload, stream download, delete. No tenant context, no database. |
| `IStorageService` | `S3StorageService` | Secure document upload — PDF magic bytes validation, filename sanitisation, security headers. |

Pick the one that matches your use case. They can all coexist; each registers under its own interface.

---

## 1. IFileStorageService — Tenant-aware R2 storage

The richest abstraction. Every operation is scoped to the authenticated tenant (resolved from JWT). File metadata is stored in PostgreSQL (`FileMetadata` table) alongside the R2 object.

See **[STORAGE.md](STORAGE.md)** for the full setup guide, API endpoint reference, and cost comparison.

### Quick start

```csharp
// Registration (called automatically from AddAppBlueprintInfrastructure)
services.AddCloudflareR2Service();
```

```bash
# Required environment variables
CLOUDFLARE_R2_ACCESSKEYID=<key-id>
CLOUDFLARE_R2_SECRETACCESSKEY=<secret>
CLOUDFLARE_R2_ENDPOINTURL=https://<account-id>.r2.cloudflarestorage.com
CLOUDFLARE_R2_PRIVATEBUCKETNAME=appblueprint-private
CLOUDFLARE_R2_PUBLICBUCKETNAME=appblueprint-public
```

```csharp
public class MyService(IFileStorageService storage)
{
    public async Task<StoredFile> UploadAsync(Stream stream, string name)
    {
        return await storage.UploadAsync(new UploadFileRequest(
            FileStream: stream,
            FileName: name,
            ContentType: "image/jpeg",
            Folder: "profile-images",
            IsPublic: true));
    }
}
```

---

## 2. IObjectStorageService — Generic S3 primitive

Stateless, no database, no tenant context. Use this when you need direct S3-compatible I/O from non-tenant code (background jobs, CLI tools, migration scripts).

Works with Cloudflare R2, AWS S3, MinIO, or any provider that exposes an S3-compatible endpoint.

### Registration

```csharp
// Called automatically from AddAppBlueprintInfrastructure, or explicitly:
services.AddObjectStorageService(configuration);
```

### Configuration

```bash
STORAGE_ENDPOINT=https://<account-id>.r2.cloudflarestorage.com
STORAGE_BUCKET_NAME=my-bucket
STORAGE_ACCESS_KEY=<key>
STORAGE_SECRET_KEY=<secret>
```

```json
// appsettings.json alternative
{
  "Storage": {
    "Endpoint": "https://<account-id>.r2.cloudflarestorage.com",
    "BucketName": "my-bucket",
    "AccessKey": "<key>",
    "SecretKey": "<secret>"
  }
}
```

### Usage

```csharp
public class BackgroundExporter(IObjectStorageService storage)
{
    public async Task ExportAsync(string key, Stream data, CancellationToken ct)
    {
        await storage.UploadAsync(key, data, "application/octet-stream", ct);
    }

    public async Task<Stream> ImportAsync(string key, CancellationToken ct)
    {
        return await storage.DownloadAsync(key, ct);
    }
}
```

---

## 3. IStorageService — Secure document upload

Designed for untrusted user input. Enforces strict PDF validation before anything reaches the bucket. All security controls are applied server-side — client-supplied Content-Type and file extensions are ignored.

### Security controls

| Control | Detail |
|---|---|
| **Magic bytes validation** | Reads first 4 bytes (`0x25 0x50 0x44 0x46` = `%PDF`). Rejects anything that does not match, regardless of extension or MIME type. |
| **Filename sanitisation** | `Path.GetFileName` strips path traversal (`../../`). Allowlist `[a-z0-9-_]`. Truncated at 100 chars. `.pdf` extension always enforced. |
| **Content-Disposition: attachment** | Prevents browsers from rendering or executing the file inline, mitigating stored-XSS in embedded PDFs. |
| **Security metadata** | `x-amz-meta-x-content-type-options: nosniff` and `x-amz-meta-content-security-policy: default-src 'none'` stored on the object. Use Cloudflare Transform Rules to promote these to actual HTTP response headers. |
| **Non-enumerable object keys** | Keys follow the pattern `documents/{uuid}/{sanitised-name}.pdf` — UUIDs prevent sequential enumeration. |

### Registration

```csharp
// Called automatically from AddAppBlueprintInfrastructure, or explicitly:
services.AddS3StorageService(configuration);
```

### Configuration

Uses the same `STORAGE_*` keys as `IObjectStorageService`:

```bash
STORAGE_ENDPOINT=https://<account-id>.r2.cloudflarestorage.com
STORAGE_BUCKET_NAME=my-documents-bucket
STORAGE_ACCESS_KEY=<key>
STORAGE_SECRET_KEY=<secret>
```

### Usage

```csharp
// Inject IStorageService
public class ContractService(IStorageService storage)
{
    public async Task<string> StoreContractAsync(Stream pdf, string name, CancellationToken ct)
    {
        // Throws InvalidFileTypeException if magic bytes do not match
        return await storage.UploadDocumentAsync(pdf, name, ct);
    }
}
```

The stream **must be seekable** (ASP.NET Core's `IFormFile.OpenReadStream()` always returns a seekable stream).

### Minimal API endpoint

`POST /api/v1/upload/document` is registered automatically via `app.MapDocumentUploadEndpoints()` in `Program.cs`.

```bash
curl -X POST https://your-api.com/api/v1/upload/document \
  -H "Authorization: Bearer <token>" \
  -F "file=@contract.pdf"
```

**Response:**
```json
{
  "objectKey": "documents/a3f1b2c4.../contract.pdf",
  "originalFileName": "contract.pdf",
  "sizeInBytes": 204800
}
```

**Error responses:**

| Status | Cause |
|---|---|
| `400` | Empty file, wrong Content-Type (soft guard), or magic bytes mismatch |
| `401` | Missing or invalid JWT |
| `413` | File exceeds 10 MB |
| `502` | S3 write failure |

### Cloudflare Transform Rules (recommended)

To serve the security metadata as real HTTP response headers, add a Transform Rule in the Cloudflare dashboard:

```
When: hostname equals your-domain.com AND URI path starts with /documents/
Then: Set response header X-Content-Type-Options = nosniff
      Set response header Content-Security-Policy = default-src 'none';
```

---

## Choosing the right abstraction

```
Need tenant isolation + metadata DB?  →  IFileStorageService
Need raw S3 I/O, no tenant context?   →  IObjectStorageService
Uploading untrusted user documents?   →  IStorageService
```

---

## Build

```powershell
cd Code\AppBlueprint\Shared-Modules\Infrastructure\AppBlueprint.Infrastructure.Storage
dotnet build
```

## Dependencies

- `AWSSDK.S3` — S3-compatible client (Cloudflare R2 uses S3 protocol)
- `AppBlueprint.Application` — interfaces and options
- `AppBlueprint.Infrastructure.Persistence` — `FileMetadataEntity` used by `R2FileStorageService`
