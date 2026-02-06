using DeploymentManager.ApiService.Domain.Enum.Pulumi;

namespace DeploymentManager.ApiService.Domain.DTOs.Project;

public class ProjectResponseDto
{
    public Guid Id { get; set; } // Unique identifier for the project
    public string Name { get; set; } // Name of the project
    public List<EnvironmentType> Environments { get; set; } = new();
    public string ApplicationName { get; set; }


    // Constructors, if necessary, for initialization
    // You might also include methods for mapping between entities and DTOs if needed
}
