using AppBlueprint.Domain.Entities.Notifications;
using AppBlueprint.Domain.Interfaces.Repositories;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public sealed class UserNotificationPreferencesRepository(BaselineDbContext context) : INotificationPreferencesRepository
{
    public async Task<NotificationPreferencesEntity?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        return await context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(NotificationPreferencesEntity preferences, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(preferences);
        await context.NotificationPreferences.AddAsync(preferences, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(NotificationPreferencesEntity preferences, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(preferences);
        context.NotificationPreferences.Update(preferences);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        NotificationPreferencesEntity? preferences = await GetByUserIdAsync(userId, cancellationToken);
        if (preferences is not null)
        {
            context.NotificationPreferences.Remove(preferences);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
