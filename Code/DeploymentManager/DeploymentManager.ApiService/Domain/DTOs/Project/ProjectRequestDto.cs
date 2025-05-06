using DeploymentPortal.ApiService.Domain.Enum.Pulumi;

namespace Domain.DTOs.Project;

public class ProjectRequestDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } // Name of the project
    public EnvironmentType Environment { get; set; }
    public string ApplicationName { get; set; }


    // Constructors, if necessary, for initialization
    // You might also include methods for mapping between entities and DTOs if needed
}
