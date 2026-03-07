using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Contracts.Baseline.Subscription.Requests;

public class CreateSubscriptionRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Code { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Status { get; set; }
}
