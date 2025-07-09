namespace AppBlueprint.Contracts.B2B.Contracts.Organization.Requests;

public class CreateOrganizationRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
