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

    public string UserId { get; set; }
    public UserEntity User { get; set; }

    public string RoleId { get; set; }
    public RoleEntity Role { get; set; }
}
