using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Contracts.Baseline.Payment.Requests;

public class CancelStripeSubscriptionRequest
{
    [Required]
    public required string SubscriptionId { get; set; }
}
