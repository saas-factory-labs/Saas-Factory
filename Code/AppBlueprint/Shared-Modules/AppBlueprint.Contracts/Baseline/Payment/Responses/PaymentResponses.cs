namespace AppBlueprint.Contracts.Baseline.Payment.Responses;

public class CustomerResponse
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SubscriptionResponse
{
    public required string Id { get; set; }
    public required string CustomerId { get; set; }
    public required string Status { get; set; }
    public required string PriceId { get; set; }
    public long Amount { get; set; }
    public required string Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CanceledAt { get; set; }
    public DateTime? CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
}

public class SubscriptionListResponse
{
    public required List<SubscriptionResponse> Subscriptions { get; set; }
    public bool HasMore { get; set; }
    public int TotalCount { get; set; }
}