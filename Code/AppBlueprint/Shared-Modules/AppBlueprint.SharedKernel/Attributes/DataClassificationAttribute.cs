using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.SharedKernel.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class DataClassificationAttribute : Attribute
{
    public DataClassificationAttribute(GDPRType classification)
    {
        Classification = classification;
    }

    public GDPRType Classification { get; }
}
