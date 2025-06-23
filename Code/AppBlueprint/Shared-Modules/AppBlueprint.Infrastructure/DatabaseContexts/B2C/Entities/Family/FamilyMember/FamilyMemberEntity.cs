using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyMember;

public class FamilyMemberEntity : BaseEntity
{
    public FamilyMemberEntity()
    {
        Id = PrefixedUlid.Generate("fmem");
        User = new UserEntity
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty,
            UserName = string.Empty,
            Profile = new ProfileEntity()
        };
        Family = new FamilyEntity
        {
            Name = string.Empty,
            Description = string.Empty,
            IsActive = true
        };
    }

    // Alias name (optional)
    public string Alias { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    // Relationships
    public string UserId { get; set; } = string.Empty;
    public UserEntity User { get; set; }

    public string FamilyId { get; set; } = string.Empty;
    public FamilyEntity Family { get; set; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string FirstName { get; set; } = string.Empty;

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string LastName { get; set; } = string.Empty;
}
