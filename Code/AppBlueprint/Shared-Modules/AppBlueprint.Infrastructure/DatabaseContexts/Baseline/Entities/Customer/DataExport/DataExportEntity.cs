namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.DataExport;

public class DataExportEntity
{
    public int Id { get; set; }
    public Uri? DownloadUrl { get; set; }
    public required string FileName { get; set; }
    public required double FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
