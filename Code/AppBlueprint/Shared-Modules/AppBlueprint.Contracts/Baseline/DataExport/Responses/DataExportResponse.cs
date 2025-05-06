using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;

namespace AppBlueprint.Contracts.Baseline.DataExport.Responses;

public class DataExportResponse
{
    public string DownloadUrl { get; set; }

    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public string FileName { get; set; }

    public string FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
}
