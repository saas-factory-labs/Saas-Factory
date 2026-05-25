# AppBlueprint.Infrastructure.Storage

Object storage infrastructure for the AppBlueprint SaaS platform using Cloudflare R2 (S3-compatible).

## What this package provides

- `ObjectStorageService` — basic S3-compatible upload/download
- `R2FileStorageService` — tenant-aware file storage with PostgreSQL metadata tracking
- `FileValidationService` — file type and size validation
- Pre-signed URL generation for private file access
- Multi-tenant file isolation via tenant-prefixed keys

## Usage

```csharp
builder.Services.AddAppBlueprintStorage(builder.Configuration);
```

Set the following environment variables or `CloudflareR2` configuration section:
- `AccessKeyId` — R2 access key
- `SecretAccessKey` — R2 secret key
- `EndpointUrl` — R2 endpoint URL
- `PrivateBucketName` / `PublicBucketName` — bucket names

## NuGet packages included

- `AWSSDK.S3`
- `AWSSDK.Core`
- `AWSSDK.SecurityToken`
