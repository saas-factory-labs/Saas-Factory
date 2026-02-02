using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Application.Options;

/// <summary>
/// Configuration options for Cloudflare R2 object storage.
/// </summary>
public sealed class CloudflareR2Options
{
    public const string SectionName = "Cloudflare:R2";
    
    /// <summary>
    /// Cloudflare R2 Access Key ID.
    /// Environment variable (development): APPBLUEPRINT_CLOUDFLARE_R2_ACCESSKEYID
    /// Environment variable (production): CLOUDFLARE_R2_ACCESSKEYID
    /// </summary>
    [Required]
    public string AccessKeyId { get; set; } = string.Empty;
    
    /// <summary>
    /// Cloudflare R2 Secret Access Key.
    /// Environment variable (development): APPBLUEPRINT_CLOUDFLARE_R2_SECRETACCESSKEY
    /// Environment variable (production): CLOUDFLARE_R2_SECRETACCESSKEY
    /// </summary>
    [Required]
    public string SecretAccessKey { get; set; } = string.Empty;
    
    /// <summary>
    /// R2 endpoint URL (e.g., https://[account-id].r2.cloudflarestorage.com).
    /// Environment variable (development): APPBLUEPRINT_CLOUDFLARE_R2_ENDPOINTURL
    /// Environment variable (production): CLOUDFLARE_R2_ENDPOINTURL
    /// </summary>
    [Required]
    [Url]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI properties should not be strings", Justification = "Needs to be string for JSON configuration binding")]
    public string EndpointUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Private bucket name for secure files (CRM documents, rental agreements, private user data).
    /// Requires pre-signed URLs for access.
    /// Environment variable (development): APPBLUEPRINT_CLOUDFLARE_R2_PRIVATEBUCKETNAME
    /// Environment variable (production): CLOUDFLARE_R2_PRIVATEBUCKETNAME
    /// </summary>
    [Required]
    public string PrivateBucketName { get; set; } = string.Empty;
    
    /// <summary>
    /// Public bucket name for publicly accessible files (dating app profile images).
    /// Files are accessible via direct URLs with GUID-based obscurity.
    /// Environment variable (development): APPBLUEPRINT_CLOUDFLARE_R2_PUBLICBUCKETNAME
    /// Environment variable (production): CLOUDFLARE_R2_PUBLICBUCKETNAME
    /// </summary>
    [Required]
    public string PublicBucketName { get; set; } = string.Empty;
    
    /// <summary>
    /// Custom domain for public bucket (optional).
    /// If set, public URLs will use this domain instead of R2 endpoint.
    /// Example: https://cdn.example.com
    /// Environment variable (development): APPBLUEPRINT_CLOUDFLARE_R2_PUBLICDOMAIN
    /// Environment variable (production): CLOUDFLARE_R2_PUBLICDOMAIN
    /// </summary>
    [Url]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI properties should not be strings", Justification = "Needs to be string for JSON configuration binding")]
    public string? PublicDomain { get; set; }
    
    /// <summary>
    /// Default bucket name for backward compatibility.
    /// Use PrivateBucketName or PublicBucketName instead.
    /// </summary>
    [Obsolete("Use PrivateBucketName or PublicBucketName instead")]
    public string BucketName
    {
        get => PrivateBucketName;
        set => PrivateBucketName = value;
    }
    
    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 300;
    
    /// <summary>
    /// Maximum image file size in megabytes (MB).
    /// Environment variable (development): APPBLUEPRINT_CLOUDFLARE_R2_MAXIMAGESIZEMBYTES
    /// Environment variable (production): CLOUDFLARE_R2_MAXIMAGESIZEMBYTES
    /// </summary>
    [Range(1, 100)]
    public int MaxImageSizeMB { get; set; } = 10;
    
    /// <summary>
    /// Maximum document file size in megabytes (MB).
    /// Environment variable (development): APPBLUEPRINT_CLOUDFLARE_R2_MAXDOCUMENTSIZEMBYTES
    /// Environment variable (production): CLOUDFLARE_R2_MAXDOCUMENTSIZEMBYTES
    /// </summary>
    [Range(1, 500)]
    public int MaxDocumentSizeMB { get; set; } = 50;
    
    /// <summary>
    /// Maximum video file size in megabytes (MB).
    /// Environment variable (development): APPBLUEPRINT_CLOUDFLARE_R2_MAXVIDEOSIZEMBYTES
    /// Environment variable (production): CLOUDFLARE_R2_MAXVIDEOSIZEMBYTES
    /// </summary>
    [Range(1, 5000)]
    public int MaxVideoSizeMB { get; set; } = 500;
    
    /// <summary>
    /// Validates the Cloudflare R2 configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AccessKeyId))
            throw new InvalidOperationException($"{SectionName}:AccessKeyId is required");
            
        if (string.IsNullOrWhiteSpace(SecretAccessKey))
            throw new InvalidOperationException($"{SectionName}:SecretAccessKey is required");
            
        if (string.IsNullOrWhiteSpace(EndpointUrl))
            throw new InvalidOperationException($"{SectionName}:EndpointUrl is required");
            
        if (!Uri.IsWellFormedUriString(EndpointUrl, UriKind.Absolute))
            throw new InvalidOperationException($"{SectionName}:EndpointUrl must be a valid absolute URL");
            
        if (string.IsNullOrWhiteSpace(PrivateBucketName))
            throw new InvalidOperationException($"{SectionName}:PrivateBucketName is required");
            
        if (string.IsNullOrWhiteSpace(PublicBucketName))
            throw new InvalidOperationException($"{SectionName}:PublicBucketName is required");
            
        if (!string.IsNullOrWhiteSpace(PublicDomain) && !Uri.IsWellFormedUriString(PublicDomain, UriKind.Absolute))
            throw new InvalidOperationException($"{SectionName}:PublicDomain must be a valid absolute URL");
            
        if (TimeoutSeconds <= 0)
            throw new InvalidOperationException($"{SectionName}:TimeoutSeconds must be positive");
            
        if (MaxImageSizeMB <= 0)
            throw new InvalidOperationException($"{SectionName}:MaxImageSizeMB must be positive");
            
        if (MaxDocumentSizeMB <= 0)
            throw new InvalidOperationException($"{SectionName}:MaxDocumentSizeMB must be positive");
            
        if (MaxVideoSizeMB <= 0)
            throw new InvalidOperationException($"{SectionName}:MaxVideoSizeMB must be positive");
    }
}
