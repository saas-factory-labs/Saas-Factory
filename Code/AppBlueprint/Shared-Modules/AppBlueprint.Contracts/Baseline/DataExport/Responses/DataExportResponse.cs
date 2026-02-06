using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Contracts.Baseline.DataExport.Responses;

public class DataExportResponse
{
    public required string Id { get; init; }

    public required Uri DownloadUrl { get; init; }

    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public required string FileName { get; init; }

    public required string FileSize { get; init; }
    public DateTime CreatedAt { get; init; }
}
