using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyInvite;
using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyMember;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family;

public class FamilyEntity
{
    public FamilyEntity()
    {
        FamilyMembers = new List<FamilyMemberEntity>();
        FamilyInvites = new List<FamilyInviteEntity>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    // Owner relationship
    public int OwnerId { get; set; }
    public UserEntity Owner { get; set; }

    // Family Members and Invites
    public List<FamilyMemberEntity> FamilyMembers { get; set; }
    public List<FamilyInviteEntity> FamilyInvites { get; set; }
}
