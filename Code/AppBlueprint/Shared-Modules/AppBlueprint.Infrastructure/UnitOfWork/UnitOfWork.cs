using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog;
using AppBlueprint.Infrastructure.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Infrastructure.Repositories.Interfaces;

namespace AppBlueprint.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _applicationDbContext;

    public UnitOfWork(ApplicationDbContext context)
    {
        _applicationDbContext = context;

    }

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

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _applicationDbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _applicationDbContext.Database.CurrentTransaction?
            .CommitAsync(cancellationToken) ?? Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _applicationDbContext.Database.CurrentTransaction?
            .RollbackAsync(cancellationToken) ?? Task.CompletedTask;
    }

    // Dispose pattern: dispose managed resources and suppress finalization
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Core dispose method.
    /// </summary>
    /// <param name="disposing">True if called from Dispose, false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _applicationDbContext.Dispose();
        }
    }

    // Finalizer calls Dispose(false)
    ~UnitOfWork()
    {
        Dispose(false);
    }
}
