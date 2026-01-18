namespace AppBlueprint.Contracts.Baseline.File.Responses;

/// <summary>
/// Response for file upload operations.
/// </summary>
public sealed class FileUploadResponse
{
    public required string Id { get; set; }
    public required string FileName { get; set; }
    public required string Url { get; set; }
    public required long Size { get; set; }
    public required string ContentType { get; set; }
    public required DateTime UploadedAt { get; set; }
}

/// <summary>
/// Response for presigned URL requests.
/// </summary>
public sealed class PresignedUrlResponse
{
    public required string Url { get; set; }
    public required DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Response for presigned upload URL requests.
/// </summary>
public sealed class PresignedUploadUrlResponse
{
    public required string UploadUrl { get; set; }
    public required string Key { get; set; }
    public required string FileId { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
