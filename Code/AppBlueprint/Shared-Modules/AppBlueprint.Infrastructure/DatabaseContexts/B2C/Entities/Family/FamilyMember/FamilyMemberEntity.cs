using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyMember;

public class FamilyMemberEntity
{
    public FamilyMemberEntity()
    {
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
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now,
            IsActive = true
        };
    }

    public int Id { get; set; }

    // Alias name (optional)
    public string Alias { get; set; }

    public bool IsActive { get; set; }

    // Relationships
    public int UserId { get; set; }
    public UserEntity User { get; set; }

    public int FamilyId { get; set; }
    public FamilyEntity Family { get; set; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string FirstName { get; set; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public string LastName { get; set; }
}
