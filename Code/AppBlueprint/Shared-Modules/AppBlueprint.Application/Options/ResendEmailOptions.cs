using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Application.Options;

/// <summary>
/// Configuration options for Resend email service.
/// </summary>
public sealed class ResendEmailOptions
{
    public const string SectionName = "Resend";
    
    /// <summary>
    /// Resend API Key.
    /// Environment variables (checked in order):
    /// 1. APPBLUEPRINT_RESEND_APIKEY (new standard)
    /// 2. RESEND_API_KEY (generic, for deployed apps)
    /// 3. APPBLUEPRINT_Resend__ApiKey (legacy dotnet format)
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Default "from" email address.
    /// Environment variables (checked in order):
    /// 1. APPBLUEPRINT_RESEND_FROMEMAIL (new standard)
    /// 2. RESEND_FROM_EMAIL (generic, for deployed apps)
    /// 3. APPBLUEPRINT_Resend__FromEmail (legacy dotnet format)
    /// </summary>
    [Required]
    [EmailAddress]
    public string FromEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// Default "from" name.
    /// Environment variables (checked in order):
    /// 1. APPBLUEPRINT_RESEND_FROMNAME (new standard)
    /// 2. RESEND_FROM_NAME (generic, for deployed apps)
    /// 3. APPBLUEPRINT_Resend__FromName (legacy dotnet format)
    /// </summary>
    public string? FromName { get; set; }
    
    /// <summary>
    /// Resend API base URL.
    /// </summary>
    [Url]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI properties should not be strings", Justification = "Needs to be string for JSON configuration binding")]
    public string BaseUrl { get; set; } = "https://api.resend.com";
    
    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// Validates the Resend email configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new InvalidOperationException($"{SectionName}:ApiKey is required");
            
        if (!ApiKey.StartsWith("re_", StringComparison.Ordinal))
            throw new InvalidOperationException($"{SectionName}:ApiKey must start with 're_'");
            
        if (string.IsNullOrWhiteSpace(FromEmail))
            throw new InvalidOperationException($"{SectionName}:FromEmail is required");
            
        if (!Uri.IsWellFormedUriString(BaseUrl, UriKind.Absolute))
            throw new InvalidOperationException($"{SectionName}:BaseUrl must be a valid absolute URL");
            
        if (TimeoutSeconds <= 0)
            throw new InvalidOperationException($"{SectionName}:TimeoutSeconds must be positive");
    }
}
