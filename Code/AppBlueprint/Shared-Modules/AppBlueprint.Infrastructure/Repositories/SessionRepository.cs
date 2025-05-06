using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SessionEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<SessionEntity>().ToListAsync(cancellationToken);
    }

    public async Task<SessionEntity> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Set<SessionEntity>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<SessionEntity> GetByKeyAsync(string sessionKey, CancellationToken cancellationToken)
    {
        return await _context.Set<SessionEntity>()
            .FirstOrDefaultAsync(s => s.SessionKey == sessionKey, cancellationToken);
    }

    public async Task AddAsync(SessionEntity session, CancellationToken cancellationToken)
    {
        await _context.Set<SessionEntity>().AddAsync(session, cancellationToken);
    }

    public async Task UpdateAsync(SessionEntity session, CancellationToken cancellationToken)
    {
        _context.Set<SessionEntity>().Update(session);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        SessionEntity? session = await _context.Set<SessionEntity>().FindAsync(new object[] { id }, cancellationToken);
        if (session is not null) _context.Set<SessionEntity>().Remove(session);
    }

    public async Task DeleteExpiredAsync(CancellationToken cancellationToken)
    {
        DateTime now = DateTime.UtcNow;
        IEnumerable<SessionEntity> expiredSessions = await _context.Set<SessionEntity>()
            .Where(s => s.ExpireDate < now)
            .ToListAsync(cancellationToken);

        _context.Set<SessionEntity>().RemoveRange(expiredSessions);
    }
}
