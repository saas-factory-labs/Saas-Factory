using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyInvite;
using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyMember;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family;

public class FamilyEntity : BaseEntity
{
    public FamilyEntity()
    {
        Id = PrefixedUlid.Generate("fam");
        FamilyMembers = new List<FamilyMemberEntity>();
        FamilyInvites = new List<FamilyInviteEntity>();
    }

    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Description { get; set; } = string.Empty;

    // Owner relationship
    public string OwnerId { get; set; } = string.Empty;
    public UserEntity Owner { get; set; }

    // Family Members and Invites
    public List<FamilyMemberEntity> FamilyMembers { get; set; }
    public List<FamilyInviteEntity> FamilyInvites { get; set; }
}
