namespace AppBlueprint.Application.Options;

/// <summary>
/// Configuration options for feature flags.
/// </summary>
public sealed class FeatureFlagsOptions
{
    public const string SectionName = "Features";
    
    /// <summary>
    /// Whether email notifications are enabled.
    /// </summary>
    public bool EnableEmailNotifications { get; set; } = true;
    
    /// <summary>
    /// Whether webhooks are enabled.
    /// </summary>
    public bool EnableWebhooks { get; set; } = true;
    
    /// <summary>
    /// Whether analytics tracking is enabled.
    /// </summary>
    public bool EnableAnalytics { get; set; } = false;
    
    /// <summary>
    /// Whether file uploads are enabled.
    /// </summary>
    public bool EnableFileUploads { get; set; } = true;
    
    /// <summary>
    /// Whether API rate limiting is enabled.
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;
    
    /// <summary>
    /// Custom feature flags dictionary.
    /// </summary>
    public Dictionary<string, bool> CustomFeatures { get; set; } = new();
    
    /// <summary>
    /// Checks if a feature is enabled.
    /// </summary>
    /// <param name="featureName">The feature name to check.</param>
    /// <returns>True if the feature is enabled; otherwise, false.</returns>
    public bool IsEnabled(string featureName)
    {
        return featureName switch
        {
            "EmailNotifications" => EnableEmailNotifications,
            "Webhooks" => EnableWebhooks,
            "Analytics" => EnableAnalytics,
            "FileUploads" => EnableFileUploads,
            "RateLimiting" => EnableRateLimiting,
            _ => CustomFeatures.GetValueOrDefault(featureName, false)
        };
    }
}
