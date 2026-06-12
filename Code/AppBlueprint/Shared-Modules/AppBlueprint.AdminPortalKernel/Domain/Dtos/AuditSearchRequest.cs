namespace AppBlueprint.AdminPortalKernel.Domain.Dtos;

/// <summary>Filters for the audit log page.</summary>
public sealed class AuditSearchRequest
{
    public string? ActionContains { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
