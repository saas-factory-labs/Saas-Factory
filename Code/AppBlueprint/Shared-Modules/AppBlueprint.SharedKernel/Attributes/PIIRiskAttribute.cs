namespace AppBlueprint.SharedKernel.Attributes;

/// <summary>
/// Marks a property as containing potential PII that should be scanned and tagged.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PIIRiskAttribute : Attribute
{
}
