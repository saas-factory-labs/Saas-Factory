using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public sealed class SessionEntity : BaseEntity
{
    public SessionEntity()
    {
        Id = PrefixedUlid.Generate("session");
    }

    [DataClassification(GDPRType.SensitiveMiscellaneous)]
    public required string SessionKey { get; set; }

    [DataClassification(GDPRType.SensitiveMiscellaneous)]
    public required string SessionData { get; set; }

    public DateTime ExpireDate { get; set; }
}
