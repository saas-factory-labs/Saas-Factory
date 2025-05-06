using AppBlueprint.Infrastructure.Repositories.Interfaces;

namespace AppBlueprint.Infrastructure.UnitOfWork;

// IDisposable
public interface IUnitOfWork
{
    ITeamRepository TeamRepository { get; }
    IAccountRepository AccountRepository { get; }
    IAdminRepository AdminRepository { get; }
    IRoleRepository RoleRepository { get; }
    IPermissionRepository PermissionRepository { get; }
    IUserRepository UserRepository { get; }
    ITenantRepository TenantRepository { get; }
    IOrganizationRepository OrganizationRepository { get; }
    INotificationRepository NotificationRepository { get; }

    IApiKeyRepository ApiKeyRepository { get; }
    // IAppProjectRepository AppProjectRepository { get; }

    Task SaveChangesAsync();
}
