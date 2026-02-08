namespace DeploymentManager.ApiService.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProjectRepository ProjectRepository { get; }

    // Add other repositories here as needed
    void SaveChanges();
}
