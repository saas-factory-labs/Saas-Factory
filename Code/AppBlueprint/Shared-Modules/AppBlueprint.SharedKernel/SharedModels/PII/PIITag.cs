using AppBlueprint.Application.Enums;

namespace AppBlueprint.SharedKernel.SharedModels.PII;

public record PIITag
{
    /// <summary>
    /// The category or label of the detected PII (e.g., "Email", "Phone", "CreditCard", "DanishCPR").
    /// </summary>
    public string Type { get; init; } = default!;
    
    /// <summary>
    /// The actual sensitive value detected.
    /// </summary>
    public string Value { get; init; } = default!;

    /// <summary>
    /// The character index where the PII starts in the input text.
    /// </summary>
    public int Start { get; init; }

    /// <summary>
    /// The character index where the PII ends in the input text.
    /// </summary>
    public int End { get; init; }

    /// <summary>
    /// The GDPR classification level from the GDPRType enum.
    /// </summary>
    public GDPRType Classification { get; init; }
}
