namespace AppBlueprint.Application.Enums;

[Flags]
public enum GDPRType
{
    NonPersonalIdentifiable = 0,
    DirectlyIdentifiable = 1,
    IndirectlyIdentifiable = 2,

    Sensitive =
        3 // sexual orientation, financial, health, political, religious, philosophical, genetic, biometric, trade union membership, criminal convictions, or data concerning a person
    // Financial = 8,
    // Health = 16
}
