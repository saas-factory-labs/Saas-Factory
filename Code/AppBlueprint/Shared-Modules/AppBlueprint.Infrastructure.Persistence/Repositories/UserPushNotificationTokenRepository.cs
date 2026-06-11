using AppBlueprint.Domain.Entities.Notifications;
using AppBlueprint.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using BaselineDbContext = AppBlueprint.Infrastructure.DatabaseContexts.Baseline.BaselineDbContext;

namespace AppBlueprint.Infrastructure.Repositories;

public sealed class UserPushNotificationTokenRepository(BaselineDbContext context) : IPushNotificationTokenRepository
{
    public async Task<PushNotificationTokenEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return await context.PushNotificationTokens
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<PushNotificationTokenEntity>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return await context.PushNotificationTokens
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PushNotificationTokenEntity>> GetActiveByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return await context.PushNotificationTokens
            .Where(t => t.UserId == userId && t.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PushNotificationTokenEntity>> GetActiveByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        return await context.PushNotificationTokens
            .Where(t => t.TenantId == tenantId && t.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<PushNotificationTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        return await context.PushNotificationTokens
            .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
    }

    public async Task AddAsync(PushNotificationTokenEntity token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);
        await context.PushNotificationTokens.AddAsync(token, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PushNotificationTokenEntity token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);
        context.PushNotificationTokens.Update(token);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        PushNotificationTokenEntity? token = await GetByIdAsync(id, cancellationToken);
        if (token is not null)
        {
            context.PushNotificationTokens.Remove(token);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeactivateByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        await context.PushNotificationTokens
            .Where(t => t.Token == token)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.IsActive, false)
                .SetProperty(t => t.LastUpdatedAt, DateTime.UtcNow), cancellationToken);
    }
}
