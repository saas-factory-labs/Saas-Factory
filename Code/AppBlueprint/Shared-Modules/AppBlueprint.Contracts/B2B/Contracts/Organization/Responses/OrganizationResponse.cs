namespace AppBlueprint.Contracts.B2B.Contracts.Organization.Responses;

public class OrganizationResponse
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
