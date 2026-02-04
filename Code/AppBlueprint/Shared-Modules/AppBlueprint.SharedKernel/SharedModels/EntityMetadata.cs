using AppBlueprint.SharedKernel.SharedModels.PII;

namespace AppBlueprint.SharedKernel.SharedModels;

/// <summary>
/// A generic container for entity metadata, including PII tags and future extensions.
/// </summary>
public record EntityMetadata
{
    /// <summary>
    /// PII detection results for this entity.
    /// </summary>
    public PIIMetadata? Pii { get; init; }
    
    // Future metadata properties can be added here
}
