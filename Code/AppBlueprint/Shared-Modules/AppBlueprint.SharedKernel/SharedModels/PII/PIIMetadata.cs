namespace AppBlueprint.SharedKernel.SharedModels.PII;

public record PIIMetadata
{
    public bool PiiDetected { get; init; }
    public List<PIITag> PiiTags { get; init; } = new();
    public ScannerInfo ScannerInfo { get; init; } = default!;
}

public record ScannerInfo
{
    public string Version { get; init; } = "1.0";
    public string Engine { get; init; } = default!;
}
