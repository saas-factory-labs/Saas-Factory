namespace AppBlueprint.Contracts.Baseline.EmailAddress.Responses;

public class EmailAddressResponse
{
    public string Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? CustomerId { get; set; }
    public string? TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
