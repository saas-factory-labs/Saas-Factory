namespace AppBlueprint.Contracts.B2B.Contracts.Organization.Responses;

public class OrganizationResponse
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastUpdatedAt { get; init; }
}
