using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.Account;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<IEnumerable<AccountEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<AccountEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<AccountEntity> GetBySlugAsync(string slug, CancellationToken cancellationToken);

    Task AddAsync(AccountEntity account, CancellationToken cancellationToken);
    Task UpdateAsync(AccountEntity account, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
