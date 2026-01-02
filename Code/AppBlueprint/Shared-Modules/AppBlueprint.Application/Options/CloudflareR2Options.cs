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
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__AccessKeyId.
    /// </summary>
    [Required]
    public string AccessKeyId { get; set; } = string.Empty;
    
    /// <summary>
    /// Cloudflare R2 Secret Access Key.
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__SecretAccessKey.
    /// </summary>
    [Required]
    public string SecretAccessKey { get; set; } = string.Empty;
    
    /// <summary>
    /// R2 endpoint URL (e.g., https://[account-id].r2.cloudflarestorage.com).
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__EndpointUrl.
    /// </summary>
    [Required]
    [Url]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI properties should not be strings", Justification = "Needs to be string for JSON configuration binding")]
    public string EndpointUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Default bucket name for file storage.
    /// Environment variable: APPBLUEPRINT_Cloudflare__R2__BucketName.
    /// </summary>
    [Required]
    public string BucketName { get; set; } = string.Empty;
    
    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;
    
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
