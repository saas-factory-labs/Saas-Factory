namespace AppBlueprint.Contracts.B2B.Contracts.Tenant.Requests;

public class CreateTenantRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
