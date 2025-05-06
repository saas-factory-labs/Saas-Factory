using Domain.Interfaces;

namespace Infrastructure.Persistence.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IProjectRepository ProjectRepository { get; }

    // Add other repositories here as needed
    void SaveChanges();
}
