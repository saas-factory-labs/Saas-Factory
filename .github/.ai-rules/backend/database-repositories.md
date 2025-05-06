<!-- # DDD Repositories

When implementing a Domain-Driven Design (DDD) Repository, follow these rules very carefully.

## Structure

Repositories should be created alongside their corresponding aggregates in the `/[scs-name]/Core/Features/[Feature]/Domain` directory.

## Implementation

1. Create an public sealed class implementation using primary constructor.
2. All implementations must inherit from `RepositoryBase<TAggregate, TId>`.
3. Create an interface that extends `IBaseRepository<TAggregate, TId>` or `ICrudRepository<TAggregate, TId>`:
   - Use `IBaseRepository` when you do not need all CRUD operations.
   - Only include methods that are needed for your specific aggregate.
4. Only return Aggregates or custom projections, but never Entities or Value Objects from a repository.
5. Never return `[PublicApi]` response DTOs.
6. Keep repositories focused on persistence operations, not business logic.
7. Repositories are automatically registered in the DI container.
8. By default Aggregates with the ITenantScopedEntity interface are automatically filtered by tenant using Entity Framework Core query filters:
   - In rare cases you may need to disable this by using the `IgnoreQueryFilters` method, e.g. when looking up an anonymous user by email.
9. Use `IEntityTypeConfiguration<TAggregate>` for Entity Framework Core model configuration (such as mapping of strongly typed IDs).

## Example 1 - Simple example with only basic operations

```csharp
public interface ILoginRepository : IAppendRepository<Login, LoginId>
{
    void Update(Login aggregate);
}

public sealed class LoginRepository(AccountManagementDbContext accountManagementDbContext)
    : RepositoryBase<Login, LoginId>(accountManagementDbContext), ILoginRepository;
```

## Example 2 - Complex example with search and pagination and using `IgnoreQueryFilters`

```csharp
public interface IUserRepository : ICrudRepository<User, UserId>
{
    Task<User?> GetByIdUnfilteredAsync(UserId id, CancellationToken cancellationToken);

    Task<User> GetLoggedInUserAsync(CancellationToken cancellationToken);

    Task<User?> GetUserByEmailUnfilteredAsync(string email, CancellationToken cancellationToken);

    Task<bool> IsEmailFreeAsync(string email, CancellationToken cancellationToken);

    Task<int> CountTenantUsersAsync(TenantId tenantId, CancellationToken cancellationToken);

    Task<(int TotalUsers, int ActiveUsers, int PendingUsers)> GetUserSummaryAsync(CancellationToken cancellationToken);

    Task<(User[] Users, int TotalItems, int TotalPages)> Search(
        string? search,
        UserRole? userRole,
        UserStatus? userStatus,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate,
        SortableUserProperties? orderBy,
        SortOrder? sortOrder,
        int? pageOffset,
        int? pageSize,
        CancellationToken cancellationToken
    );
}

internal sealed class UserRepository(AccountManagementDbContext accountManagementDbContext, IExecutionContext executionContext)
    : RepositoryBase<User, UserId>(accountManagementDbContext), IUserRepository
{
    /// <summary>
    ///     Retrieves a user by ID without applying tenant query filters.
    ///     This method should only be used during authentication processes where tenant context is not yet established.
    /// </summary>
    public async Task<User?> GetByIdUnfilteredAsync(UserId id, CancellationToken cancellationToken)
    {
        return await DbSet
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User> GetLoggedInUserAsync(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(executionContext.UserInfo.Id);
        return await GetByIdAsync(executionContext.UserInfo.Id, cancellationToken) ??
               throw new InvalidOperationException("Logged in user not found.");
    }

    /// <summary>
    ///     Retrieves a user by email without applying tenant query filters.
    ///     This method should only be used during the login processes where tenant context is not yet established.
    /// </summary>
    public async Task<User?> GetUserByEmailUnfilteredAsync(string email, CancellationToken cancellationToken)
    {
        return await DbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<bool> IsEmailFreeAsync(string email, CancellationToken cancellationToken)
    {
        return !await DbSet.AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public Task<int> CountTenantUsersAsync(TenantId tenantId, CancellationToken cancellationToken)
    {
        return DbSet.CountAsync(u => u.TenantId == tenantId, cancellationToken);
    }

    public async Task<(int TotalUsers, int ActiveUsers, int PendingUsers)> GetUserSummaryAsync(CancellationToken cancellationToken)
    {
        var thirtyDaysAgo = TimeProvider.System.GetUtcNow().AddDays(-30);

        var summary = await DbSet
            .GroupBy(_ => 1) // Group all records into a single group to calculate multiple COUNT aggregates in one query
            .Select(g => new
                {
                    TotalUsers = g.Count(),
                    ActiveUsers = g.Count(u => u.EmailConfirmed && u.ModifiedAt >= thirtyDaysAgo),
                    PendingUsers = g.Count(u => !u.EmailConfirmed)
                }
            )
            .SingleAsync(cancellationToken);

        return (summary.TotalUsers, summary.ActiveUsers, summary.PendingUsers);
    }

    public async Task<(User[] Users, int TotalItems, int TotalPages)> Search(
        string? search,
        UserRole? userRole,
        UserStatus? userStatus,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate,
        SortableUserProperties? orderBy,
        SortOrder? sortOrder,
        int? pageOffset,
        int? pageSize,
        CancellationToken cancellationToken
    )
    {
        IQueryable<User> users = DbSet;

        if (search is not null)
        {
            // Concatenate first and last name to enable searching by full name
            users = users.Where(u =>
                u.Email.Contains(search) ||
                (u.FirstName + " " + u.LastName).Contains(search) ||
                (u.Title ?? "").Contains(search)
            );
        }

        if (userRole is not null)
        {
            users = users.Where(u => u.Role == userRole);
        }

        if (userStatus is not null)
        {
            var active = userStatus == UserStatus.Active;
            users = users.Where(u => u.EmailConfirmed == active);
        }

        if (startDate is not null)
        {
            users = users.Where(u => u.ModifiedAt >= startDate);
        }

        if (endDate is not null)
        {
            users = users.Where(u => u.ModifiedAt < endDate.Value.AddDays(1));
        }

        users = orderBy switch
        {
            SortableUserProperties.CreatedAt => sortOrder == SortOrder.Ascending
                ? users.OrderBy(u => u.CreatedAt)
                : users.OrderByDescending(u => u.CreatedAt),
            SortableUserProperties.ModifiedAt => sortOrder == SortOrder.Ascending
                ? users.OrderBy(u => u.ModifiedAt)
                : users.OrderByDescending(u => u.ModifiedAt),
            SortableUserProperties.Name => sortOrder == SortOrder.Ascending
                ? users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                : users.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName),
            SortableUserProperties.Email => sortOrder == SortOrder.Ascending
                ? users.OrderBy(u => u.Email)
                : users.OrderByDescending(u => u.Email),
            SortableUserProperties.Role => sortOrder == SortOrder.Ascending
                ? users.OrderBy(u => u.Role)
                : users.OrderByDescending(u => u.Role),
            _ => users
        };

        pageSize ??= 50;
        var itemOffset = (pageOffset ?? 0) * pageSize.Value;
        var result = await users.Skip(itemOffset).Take(pageSize.Value).ToArrayAsync(cancellationToken);

        var totalItems = pageOffset == 0 && result.Length < pageSize
            ? result.Length // If the first page returns fewer items than page size, skip querying the total count
            : await users.CountAsync(cancellationToken);

        var totalPages = (totalItems - 1) / pageSize.Value + 1;
        return (result, totalItems, totalPages);
    }
}
```
## Entity Framework Core Mapping

Strongly typed IDs must be properly mapped in Entity Framework Core configurations:

```csharp
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.MapStronglyTypedUuid<User, UserId>(u => u.Id);
        builder.MapStronglyTypedLongId<User, TenantId>(u => u.TenantId);
    }
}
```

Use the appropriate mapping extension method based on the ID type:
- `MapStronglyTypedUuid` for ULIDs.
- `MapStronglyTypedLongId` for long IDs.
- `MapStronglyTypedGuid` for GUIDs. -->