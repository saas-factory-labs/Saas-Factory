namespace AppBlueprint.Contracts.Baseline.Payment.Requests;

public class CreateCustomerRequest
{
    public required string Email { get; set; }
    public required string PaymentMethodId { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
}