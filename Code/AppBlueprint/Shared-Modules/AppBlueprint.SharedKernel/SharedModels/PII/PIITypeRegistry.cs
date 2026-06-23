using System.Collections.Concurrent;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.SharedKernel.SharedModels.PII;

/// <summary>
/// A registry for PII labels and their associated GDPR classifications.
/// This acts as the authority for what "DanishPhone" or "Email" means in terms of risk.
/// </summary>
public static class PiiTypeRegistry
{
    private static readonly ConcurrentDictionary<string, PiiTypeDefinition> _registry = new(StringComparer.OrdinalIgnoreCase);

    static PiiTypeRegistry()
    {
        // Default canonical labels
        Register("Email", GDPRType.DirectlyIdentifiable);
        Register("DanishPhone", GDPRType.DirectlyIdentifiable);
        Register("InternationalPhone", GDPRType.DirectlyIdentifiable);
        Register("IPv4", GDPRType.IndirectlyIdentifiable);
        Register("IPv6", GDPRType.IndirectlyIdentifiable);
        Register("IBAN", GDPRType.Financial);
        Register("CreditCard", GDPRType.Financial);
        Register("GeoCoordinates", GDPRType.IndirectlyIdentifiable);
        Register("APIKey", GDPRType.SensitiveMiscellaneous);
        Register("DanishCPR", GDPRType.DirectlyIdentifiable);
        Register("IMEI", GDPRType.IndirectlyIdentifiable);
        Register("PotentialPasswordOrKey", GDPRType.SensitiveMiscellaneous);
    }

    public static void Register(string label, GDPRType classification, bool isCanonical = true)
    {
        _registry[label] = new PiiTypeDefinition(label, classification, isCanonical);
    }

    public static PiiTypeDefinition GetDefinition(string label)
    {
        if (_registry.TryGetValue(label, out var definition))
        {
            return definition;
        }

        // Unknown labels are treated as sensitive by default until categorized
        return new PiiTypeDefinition(label, GDPRType.SensitiveMiscellaneous, false);
    }

    public static IEnumerable<PiiTypeDefinition> GetAll()
    {
        return _registry.Values;
    }
}

public record PiiTypeDefinition(string Label, GDPRType Classification, bool IsCanonical);
