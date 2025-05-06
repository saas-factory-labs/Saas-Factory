using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface ISubscriptionRepository
{
    public Task<IEnumerable<SubscriptionEntity>> GetAllAsync(CancellationToken cancellationToken);
    public Task<SubscriptionEntity> GetByIdAsync(int id, CancellationToken cancellationToken);
    public Task AddAsync(SubscriptionEntity account, CancellationToken cancellationToken);
    public Task UpdateAsync(SubscriptionEntity account, CancellationToken cancellationToken);
    public Task DeleteAsync(int id, CancellationToken cancellationToken);
}
