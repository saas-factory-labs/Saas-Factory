using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Application.Options;

/// <summary>
/// Configuration options for Stripe payment integration.
/// </summary>
public sealed class StripeOptions
{
    public const string SectionName = "Stripe";
    
    /// <summary>
    /// Stripe API Secret Key (sk_test_... or sk_live_...).
    /// Environment variable: STRIPE_APIKEY or STRIPE_API_KEY (legacy).
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Stripe Webhook Secret for verifying webhook signatures.
    /// Environment variable: STRIPE_WEBHOOKSECRET.
    /// </summary>
    public string? WebhookSecret { get; set; }
    
    /// <summary>
    /// Webhook endpoint path.
    /// </summary>
    public string WebhookEndpoint { get; set; } = "/api/webhooks/stripe";
    
    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// Validates the Stripe configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new InvalidOperationException($"{SectionName}:ApiKey is required");
            
        if (!ApiKey.StartsWith("sk_", StringComparison.Ordinal))
            throw new InvalidOperationException($"{SectionName}:ApiKey must start with 'sk_' (secret key)");
            
        if (TimeoutSeconds <= 0)
            throw new InvalidOperationException($"{SectionName}:TimeoutSeconds must be positive");
    }
}
