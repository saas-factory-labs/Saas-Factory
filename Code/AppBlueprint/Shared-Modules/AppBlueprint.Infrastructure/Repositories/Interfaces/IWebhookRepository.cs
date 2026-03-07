using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IWebhookRepository
{
    Task<IEnumerable<WebhookEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WebhookEntity>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<WebhookEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task AddAsync(WebhookEntity webhook, CancellationToken cancellationToken = default);
    void Update(WebhookEntity webhook);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
