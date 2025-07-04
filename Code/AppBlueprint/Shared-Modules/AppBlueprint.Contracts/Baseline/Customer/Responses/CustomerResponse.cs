namespace AppBlueprint.Contracts.Baseline.Customer.Responses;

public class CustomerResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
