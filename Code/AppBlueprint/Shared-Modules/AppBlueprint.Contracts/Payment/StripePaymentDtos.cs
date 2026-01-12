using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Contracts.Payment;

/// <summary>
/// Request to create a new Stripe customer.
/// </summary>
public sealed record CreateCustomerRequest
{
    /// <summary>
    /// Customer email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
    
    /// <summary>
    /// Customer name (optional).
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// Stripe payment method ID (optional).
    /// </summary>
    public string? PaymentMethodId { get; init; }
}

/// <summary>
/// Request to create a new subscription.
/// </summary>
public sealed record CreateSubscriptionRequest
{
    /// <summary>
    /// Stripe customer ID.
    /// </summary>
    [Required]
    public string CustomerId { get; init; } = string.Empty;
    
    /// <summary>
    /// Stripe price ID from your Stripe Dashboard.
    /// </summary>
    [Required]
    public string PriceId { get; init; } = string.Empty;
}

/// <summary>
/// Request to cancel a subscription.
/// </summary>
public sealed record CancelSubscriptionRequest
{
    /// <summary>
    /// Stripe subscription ID to cancel.
    /// </summary>
    [Required]
    public string SubscriptionId { get; init; } = string.Empty;
}

/// <summary>
/// Request to create a payment intent for one-time payments.
/// </summary>
public sealed record CreatePaymentIntentRequest
{
    /// <summary>
    /// Amount in cents (e.g., 1000 = $10.00).
    /// </summary>
    [Required]
    [Range(1, long.MaxValue)]
    public long Amount { get; init; }
    
    /// <summary>
    /// Three-letter ISO currency code (e.g., "usd", "eur").
    /// </summary>
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = "usd";
    
    /// <summary>
    /// Stripe customer ID (optional).
    /// </summary>
    public string? CustomerId { get; init; }
}

/// <summary>
/// Response containing customer information.
/// </summary>
public sealed record CustomerResponse
{
    /// <summary>
    /// Stripe customer ID.
    /// </summary>
    public string Id { get; init; } = string.Empty;
    
    /// <summary>
    /// Customer email.
    /// </summary>
    public string? Email { get; init; }
    
    /// <summary>
    /// Customer name.
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// When the customer was created.
    /// </summary>
    public DateTime Created { get; init; }
}

/// <summary>
/// Response containing subscription information.
/// </summary>
public sealed record SubscriptionResponse
{
    /// <summary>
    /// Stripe subscription ID.
    /// </summary>
    public string Id { get; init; } = string.Empty;
    
    /// <summary>
    /// Stripe customer ID.
    /// </summary>
    public string CustomerId { get; init; } = string.Empty;
    
    /// <summary>
    /// Subscription status (active, past_due, canceled, etc.).
    /// </summary>
    public string Status { get; init; } = string.Empty;
    
    /// <summary>
    /// Current period start date.
    /// </summary>
    public DateTime CurrentPeriodStart { get; init; }
    
    /// <summary>
    /// Current period end date.
    /// </summary>
    public DateTime CurrentPeriodEnd { get; init; }
    
    /// <summary>
    /// Whether the subscription will cancel at period end.
    /// </summary>
    public bool CancelAtPeriodEnd { get; init; }
    
    /// <summary>
    /// When the subscription was created.
    /// </summary>
    public DateTime Created { get; init; }
}

/// <summary>
/// Response containing payment intent information.
/// </summary>
public sealed record PaymentIntentResponse
{
    /// <summary>
    /// Stripe payment intent ID.
    /// </summary>
    public string Id { get; init; } = string.Empty;
    
    /// <summary>
    /// Client secret for confirming payment on client-side.
    /// </summary>
    public string ClientSecret { get; init; } = string.Empty;
    
    /// <summary>
    /// Payment amount in cents.
    /// </summary>
    public long Amount { get; init; }
    
    /// <summary>
    /// Currency code.
    /// </summary>
    public string Currency { get; init; } = string.Empty;
    
    /// <summary>
    /// Payment intent status.
    /// </summary>
    public string Status { get; init; } = string.Empty;
}
