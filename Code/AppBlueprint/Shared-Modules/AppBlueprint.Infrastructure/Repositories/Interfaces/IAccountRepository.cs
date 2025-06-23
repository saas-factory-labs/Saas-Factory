using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IAccountRepository
{
    public Task<IEnumerable<AccountEntity>> GetAllAsync(CancellationToken cancellationToken);
    public Task<AccountEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
    public Task<AccountEntity> GetBySlugAsync(string slug, CancellationToken cancellationToken);

    public Task AddAsync(AccountEntity account, CancellationToken cancellationToken);
    public Task UpdateAsync(AccountEntity account, CancellationToken cancellationToken);
    public Task DeleteAsync(string id, CancellationToken cancellationToken);
}
