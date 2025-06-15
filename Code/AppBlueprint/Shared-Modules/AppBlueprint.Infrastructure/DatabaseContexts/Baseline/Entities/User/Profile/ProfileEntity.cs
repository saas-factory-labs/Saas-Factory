using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

public sealed class ProfileEntity
{
    public int Id { get; set; }

    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public string? PhoneNumber { get; set; }    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public string? Bio { get; set; }
    
    public Uri? AvatarUrl { get; set; }
    public Uri? WebsiteUrl { get; set; }
    public string? TimeZone { get; set; }
    public string? Language { get; set; }
    public string? Country { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
      public int UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}
