using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.DataExport;

public class DataExportEntity : BaseEntity, ITenantScoped
{
    public Uri? DownloadUrl { get; set; }
    public required string FileName { get; set; }
    public required double FileSize { get; init; }
    public string TenantId { get; set; } = string.Empty;
}
