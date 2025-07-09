namespace AppBlueprint.Contracts.Baseline.DataExport.Requests;

public class CreateDataExportRequest
{
    public Uri? DownloadUrl { get; set; }
    public required string FileName { get; set; }
    public required double FileSize { get; set; }
}
