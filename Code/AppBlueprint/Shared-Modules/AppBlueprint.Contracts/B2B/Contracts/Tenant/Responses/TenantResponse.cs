namespace AppBlueprint.Contracts.B2B.Contracts.Tenant.Responses;

public class TenantResponse
{
    public string Id { get; set; } = string.Empty;
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public bool IsSoftDeleted { get; set; }
}
