using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly ApplicationDbContext _context;

    public SubscriptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SubscriptionEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Subscriptions.ToListAsync(cancellationToken);
    }

    public async Task<SubscriptionEntity?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Subscriptions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(SubscriptionEntity subscription, CancellationToken cancellationToken)
    {
        if (subscription is null) throw new ArgumentNullException(nameof(subscription), "Subscription cannot be null.");

        await _context.Subscriptions.AddAsync(subscription, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SubscriptionEntity subscription, CancellationToken cancellationToken)
    {
        if (subscription is null) throw new ArgumentNullException(nameof(subscription), "Subscription cannot be null.");

        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        SubscriptionEntity? subscription =
            await _context.Subscriptions.FindAsync(new object[] { id }, cancellationToken);
        if (subscription is not null)
        {
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
