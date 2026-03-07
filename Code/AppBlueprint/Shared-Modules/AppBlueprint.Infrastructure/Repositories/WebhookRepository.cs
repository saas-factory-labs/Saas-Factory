using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class WebhookRepository : IWebhookRepository
{
    private readonly ApplicationDbContext _context;

    public WebhookRepository(ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<IEnumerable<WebhookEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Webhooks.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WebhookEntity>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenantId);
        return await _context.Webhooks
            .Where(w => w.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<WebhookEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _context.Webhooks.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(WebhookEntity webhook, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(webhook);
        await _context.Webhooks.AddAsync(webhook, cancellationToken);
    }

    public void Update(WebhookEntity webhook)
    {
        ArgumentNullException.ThrowIfNull(webhook);
        _context.Webhooks.Update(webhook);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        WebhookEntity? webhook = await _context.Webhooks.FindAsync([id], cancellationToken);
        if (webhook is not null)
        {
            _context.Webhooks.Remove(webhook);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}



