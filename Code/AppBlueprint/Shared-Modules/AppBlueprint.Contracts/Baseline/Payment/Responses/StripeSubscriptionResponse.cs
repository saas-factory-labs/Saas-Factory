namespace AppBlueprint.Contracts.Baseline.Payment.Responses;

public class StripeSubscriptionResponse
{
    public string SubscriptionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public DateTime? CurrentPeriodEnd { get; set; }
}
