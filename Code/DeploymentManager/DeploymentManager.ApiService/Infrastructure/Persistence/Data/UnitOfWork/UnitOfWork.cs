using DeploymentManager.ApiService.Domain.Interfaces;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.Repositories;

namespace DeploymentManager.ApiService.Infrastructure.Persistence.Data.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DeploymentManagerDbContext _context;

    public UnitOfWork(DeploymentManagerDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public IAppRepository AppRepository
    {
        get
        {
            field ??= new AppRepository(_context);
            return field;
        }
    }

    public ICustomerRepository CustomerRepository
    {
        get
        {
            field ??= new CustomerRepository(_context);
            return field;
        }
    }

    public IDeploymentRepository DeploymentRepository
    {
        get
        {
            field ??= new DeploymentRepository(_context);
            return field;
        }
    }

    public IProjectRepository ProjectRepository
    {
        get
        {
            field ??= new ProjectRepository(_context);
            return field;
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
        => _context.Dispose();
}
