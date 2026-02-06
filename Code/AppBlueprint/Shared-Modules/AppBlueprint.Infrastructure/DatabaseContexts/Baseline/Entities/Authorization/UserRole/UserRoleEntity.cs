using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.Role;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization.UserRole;

public class UserRoleEntity : BaseEntity
{
    public UserRoleEntity()
    {
        Id = PrefixedUlid.Generate("user-role");
    }

    public required string UserId { get; set; }
    public UserEntity? User { get; set; }

    public required string RoleId { get; set; }
    public RoleEntity? Role { get; set; }
}
