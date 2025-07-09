using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class UserRoleEntity : BaseEntity
{
    public UserRoleEntity()
    {
        Id = PrefixedUlid.Generate("user-role");
        User = new UserEntity
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty,
            IsActive = true,
            UserName = string.Empty,
            Profile = new ProfileEntity()
        };
        Role = new RoleEntity
        {
            Name = string.Empty,
            Description = string.Empty
        };
    }

    public required string UserId { get; set; }
    public required UserEntity User { get; set; }

    public required string RoleId { get; set; }
    public required RoleEntity Role { get; set; }
}
