namespace AppBlueprint.Contracts.B2B.Contracts.Organization.Requests;

public class UpdateOrganizationRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
