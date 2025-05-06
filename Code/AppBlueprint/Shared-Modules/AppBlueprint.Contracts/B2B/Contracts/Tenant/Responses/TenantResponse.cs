namespace AppBlueprint.Contracts.B2B.Contracts.Tenant.Responses;

public class TenantResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
