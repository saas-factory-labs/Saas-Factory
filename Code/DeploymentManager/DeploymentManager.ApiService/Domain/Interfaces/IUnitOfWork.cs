namespace DeploymentManager.ApiService.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAppRepository AppRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IDeploymentRepository DeploymentRepository { get; }
    IProjectRepository ProjectRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
