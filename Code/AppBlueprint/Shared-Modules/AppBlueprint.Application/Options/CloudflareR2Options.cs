using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Application.Options;

/// <summary>
/// Configuration options for Cloudflare R2 object storage.
/// Based on official Cloudflare R2 documentation: https://developers.cloudflare.com/r2/examples/aws/aws-sdk-net/
/// </summary>
public sealed class CloudflareR2Options
{
    public const string SectionName = "Cloudflare:R2";
    
    /// <summary>
    /// Cloudflare R2 Access Key ID.
    /// Generate via: https://developers.cloudflare.com/r2/api/tokens/
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__AccessKeyId.
    /// </summary>
    [Required]
    public string AccessKeyId { get; set; } = string.Empty;
    
    /// <summary>
    /// Cloudflare R2 Secret Access Key.
    /// Generate via: https://developers.cloudflare.com/r2/api/tokens/
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__SecretAccessKey.
    /// </summary>
    [Required]
    public string SecretAccessKey { get; set; } = string.Empty;
    
    /// <summary>
    /// R2 endpoint URL (e.g., https://[account-id].r2.cloudflarestorage.com).
    /// Find your Account ID at: https://dash.cloudflare.com/?to=/:account/workers
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__EndpointUrl.
    /// </summary>
    [Required]
    [Url]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI properties should not be strings", Justification = "Needs to be string for JSON configuration binding")]
    public string EndpointUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Cloudflare Account ID (extracted from EndpointUrl for validation).
    /// Format: https://{AccountId}.r2.cloudflarestorage.com
    /// </summary>
    [Required]
    public string AccountId { get; set; } = string.Empty;
    
    /// <summary>
    /// Default bucket name for file storage.
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__BucketName.
    /// </summary>
    [Required]
    public string BucketName { get; set; } = string.Empty;
    
    /// <summary>
    /// Public URL domain for serving files (optional, for custom domains).
    /// Example: https://files.yourapp.com
    /// If not set, uses R2 presigned URLs.
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__PublicUrlDomain.
    /// </summary>
    [Url]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI properties should not be strings", Justification = "Needs to be string for JSON configuration binding")]
    public string? PublicUrlDomain { get; set; }
    
    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// Maximum file size in bytes (default 100MB).
    /// </summary>
    [Range(1, 5_368_709_120)] // Max 5GB per Cloudflare R2 limits
    public long MaxFileSizeBytes { get; set; } = 104_857_600; // 100MB
    
    /// <summary>
    /// Enable request logging for debugging.
    /// </summary>
    public bool EnableLogging { get; set; } = false;
    
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
            
        if (string.IsNullOrWhiteSpace(BucketName))
            throw new InvalidOperationException($"{SectionName}:BucketName is required");
            
        if (TimeoutSeconds <= 0)
            throw new InvalidOperationException($"{SectionName}:TimeoutSeconds must be positive");
    }
}
