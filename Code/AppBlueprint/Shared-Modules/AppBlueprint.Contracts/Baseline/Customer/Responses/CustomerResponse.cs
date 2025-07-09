namespace AppBlueprint.Contracts.Baseline.Customer.Responses;

public class CustomerResponse
{
    public string Id { get; set; } = string.Empty;
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
