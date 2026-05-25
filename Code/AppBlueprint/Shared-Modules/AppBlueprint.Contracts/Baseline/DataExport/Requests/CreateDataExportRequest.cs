namespace AppBlueprint.Contracts.Baseline.DataExport.Requests;

/// <summary>
/// Request to create a new data export record.
/// </summary>
public class CreateDataExportRequest
{
    /// <summary>
    /// URL where the exported file can be downloaded (optional, may be generated after creation).
    /// </summary>
    public Uri? DownloadUrl { get; set; }
    
    /// <summary>
    /// Name of the exported file.
    /// </summary>
    public required string FileName { get; set; }
    
    /// <summary>
    /// Size of the exported file in bytes.
    /// </summary>
    public required double FileSize { get; set; }
}
