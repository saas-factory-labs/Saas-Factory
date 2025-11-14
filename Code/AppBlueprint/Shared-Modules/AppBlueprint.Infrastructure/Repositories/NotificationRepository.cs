using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NotificationEntity>> GetAllAsync()
    {
        return await _context.Notifications.ToListAsync();
    }
    public async Task<NotificationEntity?> GetByIdAsync(string id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task AddAsync(NotificationEntity notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public void Update(NotificationEntity notification)
    {
        _context.Notifications.Update(notification);
    }

    public void Delete(string id)
    {
        NotificationEntity? notification = _context.Notifications.Find(id);
        if (notification is not null) _context.Notifications.Remove(notification);
    }
}
