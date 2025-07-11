using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditLogEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<AuditLogEntity>().ToListAsync(cancellationToken);
    }
    public async Task<AuditLogEntity?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Set<AuditLogEntity>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(AuditLogEntity auditLog, CancellationToken cancellationToken)
    {
        await _context.Set<AuditLogEntity>().AddAsync(auditLog, cancellationToken);
    }

    public void Update(AuditLogEntity auditLog, CancellationToken cancellationToken)
    {
        _context.Set<AuditLogEntity>().Update(auditLog);
    }
    public void Delete(string id, CancellationToken cancellationToken)
    {
        AuditLogEntity? auditLog = _context.Set<AuditLogEntity>().Find(id);
        if (auditLog is not null) _context.Set<AuditLogEntity>().Remove(auditLog);
    }
}
