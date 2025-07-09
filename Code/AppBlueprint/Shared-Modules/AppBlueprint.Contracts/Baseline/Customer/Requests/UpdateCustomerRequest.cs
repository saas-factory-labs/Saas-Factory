namespace AppBlueprint.Contracts.Baseline.Customer.Requests;

public class UpdateCustomerRequest
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}
