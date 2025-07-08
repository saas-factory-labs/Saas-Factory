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
        return await _context.Set<NotificationEntity>().ToListAsync();
    }
    public async Task<NotificationEntity> GetByIdAsync(string id)
    {
        return await _context.Set<NotificationEntity>().FindAsync(id);
    }

    public async Task AddAsync(NotificationEntity notification)
    {
        await _context.Set<NotificationEntity>().AddAsync(notification);
    }

    public void Update(NotificationEntity notification)
    {
        _context.Set<NotificationEntity>().Update(notification);
    }

    public void Delete(int id)
    {
        NotificationEntity? notification = _context.Set<NotificationEntity>().Find(id);
        if (notification is not null) _context.Set<NotificationEntity>().Remove(notification);
    }
}
