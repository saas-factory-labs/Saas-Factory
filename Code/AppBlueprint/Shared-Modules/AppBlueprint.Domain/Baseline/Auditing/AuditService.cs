namespace AppBlueprint.Domain.Baseline.Auditing;

public sealed class AuditService
{
    private AuditService() { }
    
    public static Task LogChangeAsync(string entityName, Guid entityId, string action, object oldValue, object newValue, string userId)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
    
    public static Task<IEnumerable<object>> GetAuditTrailAsync(string entityName, Guid entityId)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
    
    public static Task<IEnumerable<object>> GetUserActivityAsync(string userId, DateTime fromDate, DateTime toDate)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
}
