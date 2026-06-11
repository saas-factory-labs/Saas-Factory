using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.PaymentProvider;

/// <summary>
/// Represents a payment service provider (e.g., Stripe, PayPal, Square).
/// Used as a reference table for payment processing integrations.
/// </summary>
public class PaymentProviderEntity : BaseEntity
{
    public PaymentProviderEntity()
    {
        Id = PrefixedUlid.Generate("pay_prov");
    }

    /// <summary>
    /// Name of the payment provider (e.g., Stripe, PayPal, Square)
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Optional description of the payment provider and its capabilities
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if this payment provider is currently active and available for use
    /// </summary>
    public bool IsActive { get; set; }
}
