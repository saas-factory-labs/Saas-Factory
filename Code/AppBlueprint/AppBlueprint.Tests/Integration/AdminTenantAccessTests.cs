using AppBlueprint.Application.Constants;
using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace AppBlueprint.Tests.Integration;

/// <summary>
/// Integration tests for AdminTenantAccessService.
/// Verifies that admins can only READ tenant data, never modify or delete.
/// Tests defense-in-depth: .AsNoTracking() + RLS write policies + audit logging.
/// </summary>
[Category("AdminAccess")]
[Category("Security")]
public class AdminTenantAccessTests : IAsyncDisposable
{
    private ApplicationDbContext? _dbContext;
    private AdminTenantAccessService? _adminService;
    private Mock<ICurrentUserService>? _mockCurrentUserService;
    private Mock<ILogger<AdminTenantAccessService>>? _mockLogger;

    private const string TenantA = "tenant-aaa";
    private const string TenantB = "tenant-bbb";
    private const string AdminUserId = "admin-user-123";

    [Before(Test)]
    public async Task Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"AdminAccessTest_{Guid.NewGuid()}")
            .Options;

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c.GetSection(It.IsAny<string>())).Returns(Mock.Of<IConfigurationSection>());
        var mockDbLogger = new Mock<ILogger<ApplicationDbContext>>();
        var mockTenantAccessor = new Mock<ITenantContextAccessor>();
        mockTenantAccessor.Setup(x => x.TenantId).Returns((string?)null);

        _dbContext = new ApplicationDbContext(options, mockConfig.Object, mockDbLogger.Object, mockTenantAccessor.Object);

        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockCurrentUserService.Setup(x => x.UserId).Returns(AdminUserId);
        _mockCurrentUserService.Setup(x => x.IsInRole(Roles.DeploymentManagerAdmin)).Returns(true);

        _mockLogger = new Mock<ILogger<AdminTenantAccessService>>();

        _adminService = new AdminTenantAccessService(
            _dbContext,
            _mockLogger.Object,
            _mockCurrentUserService.Object);

        await SeedTenantData();
    }

    private async Task SeedTenantData()
    {
        if (_dbContext is null)
            throw new InvalidOperationException("DbContext not initialized");

        for (int i = 1; i <= 5; i++)
        {
            _dbContext.Users.Add(new UserEntity
            {
                TenantId = TenantA,
                Email = $"userA{i}@example.com",
                FirstName = "User",
                LastName = $"A{i}",
                UserName = $"userA{i}",
                Profile = new ProfileEntity()
            });
        }

        for (int i = 1; i <= 3; i++)
        {
            _dbContext.Users.Add(new UserEntity
            {
                TenantId = TenantB,
                Email = $"userB{i}@example.com",
                FirstName = "User",
                LastName = $"B{i}",
                UserName = $"userB{i}",
                Profile = new ProfileEntity()
            });
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Test 1: Verifies that admins can access other tenant's data for read-only operations.
    /// </summary>
    [Test]
    public async Task AdminCanAccessOtherTenant()
    {
        if (_adminService is null || _dbContext is null)
            throw new InvalidOperationException("Test not properly initialized");

        var users = await _adminService.ExecuteReadOnlyAsAdminAsync(
            TenantB,
            "Test: Verify admin can access other tenant",
            async () => await _dbContext.Users
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(u => u.TenantId == TenantB)
                .ToListAsync());

        await Assert.That(users).HasCount().EqualTo(3);
        await Assert.That(users.All(u => u.TenantId == TenantB)).IsTrue();

        _mockLogger!.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ADMIN_TENANT_ACCESS")
                    && v.ToString()!.Contains(TenantB)
                    && v.ToString()!.Contains("Status=Success")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    /// <summary>
    /// Test 2: Verifies that non-admin users cannot use admin access service.
    /// </summary>
    [Test]
    public async Task NonAdminCannotAccessOtherTenant()
    {
        if (_mockCurrentUserService is null || _adminService is null || _dbContext is null)
            throw new InvalidOperationException("Test not properly initialized");

        _mockCurrentUserService.Setup(x => x.IsInRole(Roles.DeploymentManagerAdmin)).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _adminService.ExecuteReadOnlyAsAdminAsync(
                TenantB,
                "Should fail",
                async () => await _dbContext.Users
                    .AsNoTracking()
                    .IgnoreQueryFilters()
                    .ToListAsync()));

        _mockLogger!.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ADMIN_ACCESS_DENIED")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 3: Verifies that admin access creates comprehensive audit log entries.
    /// </summary>
    [Test]
    public async Task AdminAccessCreatesAuditLog()
    {
        if (_adminService is null || _dbContext is null)
            throw new InvalidOperationException("Test not properly initialized");

        string reason = "Support ticket #12345 - User login issue";

        await _adminService.ExecuteReadOnlyAsAdminAsync(
            TenantB,
            reason,
            async () => await _dbContext.Users
                .AsNoTracking()
                .IgnoreQueryFilters()
                .ToListAsync());

        _mockLogger!.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("ADMIN_TENANT_ACCESS") &&
                    v.ToString()!.Contains($"AdminUserId={AdminUserId}") &&
                    v.ToString()!.Contains($"TenantId={TenantB}") &&
                    v.ToString()!.Contains($"Reason={reason}") &&
                    v.ToString()!.Contains("Status=Attempting")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("ADMIN_TENANT_ACCESS") &&
                    v.ToString()!.Contains($"AdminUserId={AdminUserId}") &&
                    v.ToString()!.Contains($"TenantId={TenantB}") &&
                    v.ToString()!.Contains($"Reason={reason}") &&
                    v.ToString()!.Contains("Status=Success")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 4: Verifies that admins CANNOT modify tenant data (read-only enforcement).
    /// This tests defense-in-depth: .AsNoTracking() prevents EF Core change tracking.
    /// </summary>
    [Test]
    public async Task AdminCannotModifyTenantData()
    {
        if (_adminService is null || _dbContext is null)
            throw new InvalidOperationException("Test not properly initialized");

        UserEntity? user = await _adminService.ExecuteReadOnlyAsAdminAsync(
            TenantA,
            "Test: Verify admin cannot modify data",
            async () => await _dbContext.Users
                .AsNoTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.TenantId == TenantA));

        await Assert.That(user).IsNotNull();

        string originalEmail = user!.Email!;

        user.Email = "hacked@example.com";

        await _dbContext.SaveChangesAsync();

        var verifyUser = await _dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        await Assert.That(verifyUser).IsNotNull();
        await Assert.That(verifyUser!.Email).IsEqualTo(originalEmail);
        await Assert.That(verifyUser.Email).IsNotEqualTo("hacked@example.com");
    }

    /// <summary>
    /// Test 5: Verifies that reason is mandatory for admin access (audit requirement).
    /// </summary>
    [Test]
    public async Task AdminAccessRequiresReason()
    {
        if (_adminService is null || _dbContext is null)
            throw new InvalidOperationException("Test not properly initialized");

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _adminService.ExecuteReadOnlyAsAdminAsync(
                TenantB,
                "",
                async () => await _dbContext.Users
                    .AsNoTracking()
                    .ToListAsync()));

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _adminService.ExecuteReadOnlyAsAdminAsync(
                TenantB,
                null!,
                async () => await _dbContext.Users
                    .AsNoTracking()
                    .ToListAsync()));
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContext is not null)
        {
            await _dbContext.DisposeAsync();
        }
    }
}
