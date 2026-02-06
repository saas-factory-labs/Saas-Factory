namespace AppBlueprint.Contracts.B2B.Contracts.Tenant.Responses;

public class TenantResponse
{
    public string Id { get; init; } = string.Empty;
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastUpdatedAt { get; init; }
    public bool IsSoftDeleted { get; init; }
}
