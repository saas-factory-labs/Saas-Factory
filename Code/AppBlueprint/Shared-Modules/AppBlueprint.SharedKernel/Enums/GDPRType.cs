namespace AppBlueprint.Application.Enums;

[Flags]
public enum GDPRType
{
    None = 0,
    NonPersonalIdentifiable = 1,
    DirectlyIdentifiable = 2,
    IndirectlyIdentifiable = 3,
    SexualOrientation = 4,
    Financial = 5,
    Health = 6,
    Political = 7,
    Religious = 8,
    Criminal = 9,
        SensitiveMiscellaneous = 10
}
