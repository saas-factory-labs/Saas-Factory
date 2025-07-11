using AppBlueprint.Application.Enums;

namespace AppBlueprint.Application.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class DataClassificationAttribute : Attribute
{
    public DataClassificationAttribute(GDPRType classification)
    {
        Classification = classification;
    }

    public GDPRType Classification { get; }
}
