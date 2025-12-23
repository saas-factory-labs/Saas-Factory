using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
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
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"AdminAccessTest_{Guid.NewGuid()}")
            .Options;

        // Mock ITenantContextAccessor (not used during admin access, but required by DbContext)
        var mockTenantAccessor = new Mock<Infrastructure.Services.ITenantContextAccessor>();
        mockTenantAccessor.Setup(x => x.TenantId).Returns((string?)null);

        _dbContext = new ApplicationDbContext(options, mockTenantAccessor.Object);

        // Mock ICurrentUserService for admin role verification
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockCurrentUserService.Setup(x => x.UserId).Returns(AdminUserId);
        _mockCurrentUserService.Setup(x => x.IsInRole("SuperAdmin")).Returns(true);

        // Mock logger to verify audit logging
        _mockLogger = new Mock<ILogger<AdminTenantAccessService>>();

        // Create service under test
        _adminService = new AdminTenantAccessService(
            _dbContext,
            _mockLogger.Object,
            _mockCurrentUserService.Object);

        // Seed test data
        await SeedTenantData();
    }

    private async Task SeedTenantData()
    {
        if (_dbContext is null)
            throw new InvalidOperationException("DbContext not initialized");

        // Seed tenant A users
        for (int i = 1; i <= 5; i++)
        {
            _dbContext.Users.Add(UserEntity.Create(
                TenantA,
                $"userA{i}@example.com",
                UserRole.User));
        }

        // Seed tenant B users
        for (int i = 1; i <= 3; i++)
        {
            _dbContext.Users.Add(UserEntity.Create(
                TenantB,
                $"userB{i}@example.com",
                UserRole.User));
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Test 1: Verifies that admins can access other tenant's data for read-only operations.
    /// </summary>
    [Test]
    public async Task AdminCanAccessOtherTenant()
    {
        // Arrange
        if (_adminService is null || _dbContext is null)
            throw new InvalidOperationException("Test not properly initialized");

        // Act - Admin accesses tenant B's data
        var users = await _adminService.ExecuteReadOnlyAsAdminAsync(
            TenantB,
            "Test: Verify admin can access other tenant",
            async () => await _dbContext.Users
                .AsNoTracking() // ✅ Read-only
                .IgnoreQueryFilters()
                .Where(u => u.TenantId == TenantB)
                .ToListAsync());

        // Assert - Should retrieve all tenant B users
        await Assert.That(users).HasCount().EqualTo(3);
        await Assert.That(users).IsAllSatisfy(u => u.TenantId == TenantB);
        
        // Verify audit log created
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
        // Arrange - User is NOT a SuperAdmin
        if (_mockCurrentUserService is null || _adminService is null || _dbContext is null)
            throw new InvalidOperationException("Test not properly initialized");

        _mockCurrentUserService.Setup(x => x.IsInRole("SuperAdmin")).Returns(false);

        // Act & Assert - Should throw UnauthorizedAccessException
        await Assert.That(async () => await _adminService.ExecuteReadOnlyAsAdminAsync(
            TenantB,
            "Should fail",
            async () => await _dbContext.Users
                .AsNoTracking()
                .IgnoreQueryFilters()
                .ToListAsync()
        )).ThrowsExactly<UnauthorizedAccessException>();

        // Verify access denied was logged
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
        // Arrange
        if (_adminService is null || _dbContext is null)
            throw new InvalidOperationException("Test not properly initialized");

        string reason = "Support ticket #12345 - User login issue";

        // Act - Admin accesses tenant data
        await _adminService.ExecuteReadOnlyAsAdminAsync(
            TenantB,
            reason,
            async () => await _dbContext.Users
                .AsNoTracking()
                .IgnoreQueryFilters()
                .ToListAsync());

        // Assert - Verify audit log contains:
        // 1. Admin user ID
        // 2. Target tenant ID
        // 3. Reason for access
        // 4. Status (Attempting, Success)
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
    /// In production, RLS write policies would also block at database level.
    /// </summary>
    [Test]
    public async Task AdminCannotModifyTenantData()
    {
        // Arrange
        if (_adminService is null || _dbContext is null)
            throw new InvalidOperationException("Test not properly initialized");

        // Act - Admin retrieves user with AsNoTracking (read-only)
        UserEntity? user = await _adminService.ExecuteReadOnlyAsAdminAsync(
            TenantA,
            "Test: Verify admin cannot modify data",
            async () => await _dbContext.Users
                .AsNoTracking() // ✅ This prevents change tracking
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.TenantId == TenantA));

        // Assert - User retrieved successfully
        await Assert.That(user).IsNotNull();
        
        string originalEmail = user!.Email;

        // Attempt to modify the entity
        user.UpdateEmail("hacked@example.com");

        // Try to save changes (should have no effect because entity is not tracked)
        await _dbContext.SaveChangesAsync();

        // Verify data was NOT modified in database
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
        // Arrange
        if (_adminService is null || _dbContext is null)
            throw new InvalidOperationException("Test not properly initialized");

        // Act & Assert - Empty reason should throw ArgumentException
        await Assert.That(async () => await _adminService.ExecuteReadOnlyAsAdminAsync(
            TenantB,
            "", // ❌ Empty reason
            async () => await _dbContext.Users
                .AsNoTracking()
                .ToListAsync()
        )).ThrowsExactly<ArgumentException>();

        // Null reason should throw ArgumentNullException
        await Assert.That(async () => await _adminService.ExecuteReadOnlyAsAdminAsync(
            TenantB,
            null!, // ❌ Null reason
            async () => await _dbContext.Users
                .AsNoTracking()
                .ToListAsync()
        )).ThrowsExactly<ArgumentNullException>();
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContext is not null)
        {
            await _dbContext.DisposeAsync();
        }
    }
}
