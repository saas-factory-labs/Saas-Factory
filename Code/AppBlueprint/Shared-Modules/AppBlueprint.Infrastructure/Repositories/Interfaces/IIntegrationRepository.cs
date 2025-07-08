using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Integration;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IIntegrationRepository
{
    Task<IEnumerable<IntegrationEntity>> GetAllAsync(CancellationToken cancellationToken); Task<IntegrationEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task AddAsync(IntegrationEntity integration, CancellationToken cancellationToken);
    Task UpdateAsync(IntegrationEntity integration, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
