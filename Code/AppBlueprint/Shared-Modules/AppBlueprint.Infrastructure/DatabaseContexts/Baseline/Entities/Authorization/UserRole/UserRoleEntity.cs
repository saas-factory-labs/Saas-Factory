using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class UserRoleEntity
{
    public UserRoleEntity()
    {
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
            Description = string.Empty,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        };
    }    public int Id { get; set; }
    
    public int UserId { get; set; }
    public UserEntity User { get; set; }

    public int RoleId { get; set; }
    public RoleEntity Role { get; set; }
}
