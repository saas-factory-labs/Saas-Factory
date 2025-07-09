using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyInvite;
using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family.FamilyMember;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities.Family;

public sealed class FamilyEntity : BaseEntity
{
    private readonly List<FamilyMemberEntity> _familyMembers = new();
    private readonly List<FamilyInviteEntity> _familyInvites = new();

    public FamilyEntity()
    {
        Id = PrefixedUlid.Generate("fam");
        Name = string.Empty;
        Description = string.Empty;
        OwnerId = string.Empty;
    }

    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Description { get; set; } = string.Empty;

    // Owner relationship
    public string OwnerId { get; set; } = string.Empty;
    public UserEntity? Owner { get; set; }

    // Family Members and Invites
    public IReadOnlyCollection<FamilyMemberEntity> FamilyMembers => _familyMembers.AsReadOnly();
    public IReadOnlyCollection<FamilyInviteEntity> FamilyInvites => _familyInvites.AsReadOnly();

    // Domain methods for controlled collection management
    public void AddFamilyMember(FamilyMemberEntity familyMember)
    {
        ArgumentNullException.ThrowIfNull(familyMember);

        if (_familyMembers.Any(fm => fm.Id == familyMember.Id))
            return; // Family member already exists

        _familyMembers.Add(familyMember);
    }

    public void RemoveFamilyMember(FamilyMemberEntity familyMember)
    {
        ArgumentNullException.ThrowIfNull(familyMember);
        _familyMembers.Remove(familyMember);
    }

    public void AddFamilyInvite(FamilyInviteEntity familyInvite)
    {
        ArgumentNullException.ThrowIfNull(familyInvite);

        if (_familyInvites.Any(fi => fi.Id == familyInvite.Id))
            return; // Family invite already exists

        _familyInvites.Add(familyInvite);
    }

    public void RemoveFamilyInvite(FamilyInviteEntity familyInvite)
    {
        ArgumentNullException.ThrowIfNull(familyInvite);
        _familyInvites.Remove(familyInvite);
    }
}
