using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

public sealed class ProfileEntity : BaseEntity
{    public ProfileEntity()
    {
        Id = PrefixedUlid.Generate("profile");
        UserId = string.Empty;
    }

    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public string? PhoneNumber { get; set; }

    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public string? Bio { get; set; }
    
    public Uri? AvatarUrl { get; set; }
    public Uri? WebsiteUrl { get; set; }
    public string? TimeZone { get; set; }
    public string? Language { get; set; }
    public string? Country { get; set; }

    public string UserId { get; set; }
    public UserEntity? User { get; set; }
}
