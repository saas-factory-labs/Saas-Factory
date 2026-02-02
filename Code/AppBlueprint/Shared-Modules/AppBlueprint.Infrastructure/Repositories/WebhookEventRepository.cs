using AppBlueprint.Domain.Entities.Webhooks;
using AppBlueprint.Domain.Interfaces.Repositories;
using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

/// <summary>
/// Repository for webhook event entities.
/// </summary>
public sealed class WebhookEventRepository : IWebhookEventRepository
{
    private readonly ApplicationDbContext _dbContext;

    public WebhookEventRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<WebhookEventEntity?> GetByEventIdAsync(string eventId, string source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventId);
        ArgumentNullException.ThrowIfNull(source);

        return await _dbContext.Set<WebhookEventEntity>()
            .Where(e => e.EventId == eventId && e.Source == source)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WebhookEventEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await _dbContext.Set<WebhookEventEntity>()
            .FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WebhookEventEntity>> GetByTenantIdAsync(
        string tenantId,
        int pageSize = 50,
        int pageNumber = 1,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenantId);

        return await _dbContext.Set<WebhookEventEntity>()
            .Where(e => e.TenantId == tenantId)
            .OrderByDescending(e => e.ReceivedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WebhookEventEntity>> GetByStatusAsync(
        WebhookEventStatus status,
        int maxResults = 100,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<WebhookEventEntity>()
            .Where(e => e.Status == status)
            .OrderByDescending(e => e.ReceivedAt)
            .Take(maxResults)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WebhookEventEntity>> GetFailedEventsForRetryAsync(
        int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<WebhookEventEntity>()
            .Where(e => (e.Status == WebhookEventStatus.Failed || e.Status == WebhookEventStatus.PendingRetry)
                        && e.RetryCount < maxRetries)
            .OrderBy(e => e.ReceivedAt)
            .Take(100) // Limit to 100 events per retry batch
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WebhookEventEntity> AddAsync(WebhookEventEntity webhookEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(webhookEvent);

        await _dbContext.Set<WebhookEventEntity>().AddAsync(webhookEvent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return webhookEvent;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(WebhookEventEntity webhookEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(webhookEvent);

        _dbContext.Set<WebhookEventEntity>().Update(webhookEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteOldEventsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        IEnumerable<WebhookEventEntity> oldEvents = await _dbContext.Set<WebhookEventEntity>()
            .Where(e => e.ReceivedAt < olderThan)
            .ToListAsync(cancellationToken);

        _dbContext.Set<WebhookEventEntity>().RemoveRange(oldEvents);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
