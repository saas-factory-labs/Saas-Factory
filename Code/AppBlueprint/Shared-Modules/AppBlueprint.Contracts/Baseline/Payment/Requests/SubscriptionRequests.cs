namespace AppBlueprint.Contracts.Baseline.Payment.Requests;

public class CreateSubscriptionRequest
{
    public required string CustomerId { get; set; }
    public required string PriceId { get; set; }
}

public class CancelSubscriptionRequest
{
    public required string SubscriptionId { get; set; }
}