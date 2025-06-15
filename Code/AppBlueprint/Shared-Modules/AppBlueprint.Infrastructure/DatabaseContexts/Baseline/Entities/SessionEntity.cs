using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public sealed class SessionEntity
{
    public int Id { get; set; }

    [DataClassification(GDPRType.Sensitive)]
    public required string SessionKey { get; set; }
    
    [DataClassification(GDPRType.Sensitive)]
    public required string SessionData { get; set; }
    
    public DateTime ExpireDate { get; set; }
}
