namespace AppBlueprint.Contracts.B2B.Contracts.Organization.Requests;

public class UpdateOrganizationRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}
