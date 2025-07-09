namespace AppBlueprint.Contracts.Baseline.DataExport.Requests;

public class UpdateDataExportRequest
{
    public Uri? DownloadUrl { get; set; }
    public required string FileName { get; set; }
    public required double FileSize { get; set; }
}
