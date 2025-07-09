namespace AppBlueprint.Contracts.Baseline.EmailAddress.Responses;

public class EmailAddressResponse
{
    public required string Address { get; set; }
    public required string UserId { get; set; }
    public required string CustomerId { get; set; }
    public required string TenantId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
