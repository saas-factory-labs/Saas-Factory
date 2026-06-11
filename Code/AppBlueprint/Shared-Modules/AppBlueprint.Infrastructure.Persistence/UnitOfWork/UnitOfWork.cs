using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using DomainNotificationRepository = AppBlueprint.Domain.Interfaces.Repositories.INotificationRepository;

namespace AppBlueprint.Infrastructure.UnitOfWork;

public sealed class UnitOfWorkImplementation : IUnitOfWork
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly B2BDbContext _b2bDbContext;
    private readonly IDbContextFactory<ApplicationDbContext> _applicationDbContextFactory;

    public UnitOfWorkImplementation(
        ApplicationDbContext context,
        B2BDbContext b2bDbContext,
        IDbContextFactory<ApplicationDbContext> applicationDbContextFactory)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(b2bDbContext);
        ArgumentNullException.ThrowIfNull(applicationDbContextFactory);

        _applicationDbContext = context;
        _b2bDbContext = b2bDbContext;
        _applicationDbContextFactory = applicationDbContextFactory;
    }

    public IAccountRepository AccountRepository
    {
        get
        {
            field ??= new AccountRepository(_applicationDbContext);
            return field;
        }
    }

    public IAdminRepository AdminRepository
    {
        get
        {
            field ??= new AdminRepository(_applicationDbContext);
            return field;
        }
    }

    public IRoleRepository RoleRepository
    {
        get
        {
            field ??= new RoleRepository(_applicationDbContext);
            return field;
        }
    }

    public IPermissionRepository PermissionRepository
    {
        get
        {
            field ??= new PermissionRepository(_applicationDbContext);
            return field;
        }
    }

    public IUserRepository UserRepository
    {
        get
        {
            field ??= new UserRepository(_applicationDbContext);
            return field;
        }
    }

    public ITenantRepository TenantRepository
    {
        get
        {
            field ??= new TenantRepository(_applicationDbContextFactory);
            return field;
        }
    }

    public IOrganizationRepository OrganizationRepository
    {
        get
        {
            field ??= new OrganizationRepository(_b2bDbContext);
            return field;
        }
    }

    public DomainNotificationRepository NotificationRepository
    {
        get
        {
            field ??= new UserNotificationRepository(_applicationDbContext);
            return field;
        }
    }

    public IApiKeyRepository ApiKeyRepository
    {
        get
        {
            field ??= new ApiKeyRepository(_b2bDbContext);
            return field;
        }
    }

    public ITeamRepository TeamRepository
    {
        get
        {
            field ??= new TeamRepository(_b2bDbContext);
            return field;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Save changes to both contexts and return total affected rows
        var applicationChanges = await _applicationDbContext.SaveChangesAsync(cancellationToken);
        var b2bChanges = await _b2bDbContext.SaveChangesAsync(cancellationToken);
        return applicationChanges + b2bChanges;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _applicationDbContext.Database.BeginTransactionAsync(cancellationToken);
        await _b2bDbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_applicationDbContext.Database.CurrentTransaction != null)
            await _applicationDbContext.Database.CurrentTransaction.CommitAsync(cancellationToken);

        if (_b2bDbContext.Database.CurrentTransaction != null)
            await _b2bDbContext.Database.CurrentTransaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_applicationDbContext.Database.CurrentTransaction != null)
            await _applicationDbContext.Database.CurrentTransaction.RollbackAsync(cancellationToken);

        if (_b2bDbContext.Database.CurrentTransaction != null)
            await _b2bDbContext.Database.CurrentTransaction.RollbackAsync(cancellationToken);
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
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _applicationDbContext.Dispose();
            _b2bDbContext.Dispose();
        }
    }

    // Finalizer calls Dispose(false)
    ~UnitOfWorkImplementation()
    {
        Dispose(false);
    }
}
