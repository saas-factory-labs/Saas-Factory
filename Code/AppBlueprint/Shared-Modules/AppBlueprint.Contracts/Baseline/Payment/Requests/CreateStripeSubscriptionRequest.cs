using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Contracts.Baseline.Payment.Requests;

public class CreateStripeSubscriptionRequest
{
    [Required]
    public required string CustomerId { get; set; }

    [Required]
    public required string PriceId { get; set; }
}
