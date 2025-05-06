using AppBlueprint.Infrastructure.DatabaseContexts.Modules.Credit;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface ICreditRepository
{
    Task<IEnumerable<CreditEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<CreditEntity> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(CreditEntity credit, CancellationToken cancellationToken);
    Task UpdateAsync(CreditEntity credit, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<decimal> GetRemainingCreditAsync(int id, CancellationToken cancellationToken);
    Task UpdateRemainingCreditAsync(int id, decimal amount, CancellationToken cancellationToken);
}
