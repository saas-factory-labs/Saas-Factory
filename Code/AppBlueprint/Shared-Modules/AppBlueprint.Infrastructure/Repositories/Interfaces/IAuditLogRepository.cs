using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task<IEnumerable<AuditLogEntity>> GetAllAsync(CancellationToken cancellationToken); Task<AuditLogEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task AddAsync(AuditLogEntity auditLog, CancellationToken cancellationToken);
    void Update(AuditLogEntity auditLog, CancellationToken cancellationToken);
    void Delete(string id, CancellationToken cancellationToken);
}
