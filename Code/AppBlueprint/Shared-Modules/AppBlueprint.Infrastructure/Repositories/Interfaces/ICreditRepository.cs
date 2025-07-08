using AppBlueprint.Infrastructure.DatabaseContexts.Modules.Credit;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface ICreditRepository
{
    Task<IEnumerable<CreditEntity>> GetAllAsync(CancellationToken cancellationToken); Task<CreditEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task AddAsync(CreditEntity credit, CancellationToken cancellationToken);
    Task UpdateAsync(CreditEntity credit, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
    Task<decimal> GetRemainingCreditAsync(string id, CancellationToken cancellationToken);
    Task UpdateRemainingCreditAsync(string id, decimal amount, CancellationToken cancellationToken);
}
