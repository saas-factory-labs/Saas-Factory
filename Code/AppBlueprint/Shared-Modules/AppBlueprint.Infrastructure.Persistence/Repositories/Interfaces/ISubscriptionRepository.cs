using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface ISubscriptionRepository
{
    Task<IEnumerable<SubscriptionEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<SubscriptionEntity?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task AddAsync(SubscriptionEntity subscription, CancellationToken cancellationToken);
    Task UpdateAsync(SubscriptionEntity subscription, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
