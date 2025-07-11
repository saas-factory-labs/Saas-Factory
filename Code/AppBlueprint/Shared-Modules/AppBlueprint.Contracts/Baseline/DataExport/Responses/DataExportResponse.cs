using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;

namespace AppBlueprint.Contracts.Baseline.DataExport.Responses;

public class DataExportResponse
{
    public required string Id { get; set; }

    public required Uri DownloadUrl { get; set; }

    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public required string FileName { get; set; }

    public required string FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
}
