using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Infrastructure.Repositories.Interfaces;

namespace AppBlueprint.Infrastructure.UnitOfWork;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _applicationDbContext = context;
    
    private IAdminRepository? __adminRepository;    
    private IAccountRepository? _accountRepository;    
    private IApiKeyRepository? _apiKeyRepository;
    private INotificationRepository? _notificationRepository;
    private IOrganizationRepository? _organizationRepository;
    private IPermissionRepository? _permissionRepository;
    private IRoleRepository? _roleRepository;
    private ITeamRepository? _teamRepository;
    private ITenantRepository? _tenantRepository;
    private IUserRepository? _userRepository;

    public IAccountRepository AccountRepository
    {
        get
        {
            if (_accountRepository is null) _accountRepository = new AccountRepository(_applicationDbContext);

            return _accountRepository;
        }
    }

    public IAdminRepository AdminRepository
    {
        get
        {
            if (__adminRepository is null) __adminRepository = new AdminRepository(_applicationDbContext);

            return __adminRepository;
        }
    }

    public IRoleRepository RoleRepository
    {
        get
        {
            if (_roleRepository is null) _roleRepository = new RoleRepository(_applicationDbContext);

            return _roleRepository;
        }
    }

    public IPermissionRepository PermissionRepository
    {
        get
        {
            if (_permissionRepository is null) _permissionRepository = new PermissionRepository(_applicationDbContext);

            return _permissionRepository;
        }
    }

    public IUserRepository UserRepository
    {
        get
        {
            if (_userRepository is null) _userRepository = new UserRepository(_applicationDbContext);

            return _userRepository;
        }
    }

    public ITenantRepository TenantRepository
    {
        get
        {
            if (_tenantRepository is null) _tenantRepository = new TenantRepository(_applicationDbContext);

            return _tenantRepository;
        }
    }

    public IOrganizationRepository OrganizationRepository
    {
        get
        {
            if (_organizationRepository is null)
                _organizationRepository = new OrganizationRepository(_applicationDbContext);

            return _organizationRepository;
        }
    }

    public INotificationRepository NotificationRepository
    {
        get
        {
            if (_notificationRepository is null)
                _notificationRepository = new NotificationRepository(_applicationDbContext);

            return _notificationRepository;
        }
    }

    public IApiKeyRepository ApiKeyRepository
    {
        get
        {
            if (_apiKeyRepository is null) _apiKeyRepository = new ApiKeyRepository(_applicationDbContext);

            return _apiKeyRepository;
        }
    }

    public ITeamRepository TeamRepository
    {
        get
        {
            if (_teamRepository is null) _teamRepository = new TeamRepository(_applicationDbContext);

            return _teamRepository;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _applicationDbContext.SaveChangesAsync();
    }
}
