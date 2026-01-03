using AppBlueprint.Application.Services;
using AppBlueprint.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace AppBlueprint.Tests.Application.Services;

/// <summary>
/// Unit and integration tests for SignupService.
/// Tests the secure signup stored procedure integration.
/// </summary>
public sealed class SignupServiceTests : IAsyncDisposable
{
    private readonly IDbContextFactory<DbContext> _contextFactory;
    private readonly ILogger<SignupService> _logger;
    private SignupService? _signupService;

    public SignupServiceTests()
    {
        // Setup in-memory or test database context factory
        // For real integration tests, use PostgreSQL test container
        _contextFactory = null!; // TODO: Create test context factory
        _logger = null!; // TODO: Create test logger
    }

    [Before(Test)]
    public async Task Setup()
    {
        _signupService = new SignupService(_contextFactory, _logger);
        
        // TODO: Setup test database with stored procedure
        // await ExecuteSqlAsync("DROP TABLE IF EXISTS \"SignupAuditLog\";");
        // await ExecuteSqlAsync(File.ReadAllText("CreateSignupStoredProcedure.sql"));
    }

    [After(Test)]
    public async Task Cleanup()
    {
        // Cleanup test data
        await Task.CompletedTask;
    }

    [Test]
    [Skip("Requires PostgreSQL test container - enable for integration tests")]
    public async Task CreateTenantAndUserAsync_ValidRequest_CreatesSuccessfully()
    {
        // Arrange
        SignupRequest request = new()
        {
            TenantId = PrefixedUlid.Generate("tenant"),
            TenantName = "Test User",
            UserId = PrefixedUlid.Generate("user"),
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            ExternalAuthId = "logto-sub-12345",
            IpAddress = "192.168.1.1",
            UserAgent = "Mozilla/5.0 Test"
        };

        // Act
        SignupResult result = await _signupService!.CreateTenantAndUserAsync(request);
    
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.TenantId).IsEqualTo(request.TenantId);
        await Assert.That(result.UserId).IsEqualTo(request.UserId);
        await Assert.That(result.Email).IsEqualTo(request.Email);
        await Assert.That(result.ProfileId).IsNotNull();
        
        // Verify tenant was created in database
        await using DbContext context = await _contextFactory.CreateDbContextAsync();
        NpgsqlConnection? connection = context.Database.GetDbConnection() as NpgsqlConnection;
        await connection!.OpenAsync();
        
        await using NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM \"Tenants\" WHERE \"Id\" = @id";
        cmd.Parameters.AddWithValue("@id", request.TenantId);
        
        long tenantCount = (long)(await cmd.ExecuteScalarAsync())!;
        await Assert.That(tenantCount).IsEqualTo(1L);
        
        // Verify user was created in database
        cmd.CommandText = "SELECT COUNT(*) FROM \"Users\" WHERE \"Id\" = @id";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@id", request.UserId);
        
        long userCount = (long)(await cmd.ExecuteScalarAsync())!;
        await Assert.That(userCount).IsEqualTo(1L);
        
        // Verify audit log entry
        cmd.CommandText = "SELECT \"Success\" FROM \"SignupAuditLog\" WHERE \"Email\" = @email ORDER BY \"CreatedAt\" DESC LIMIT 1";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@email", request.Email);
        
        bool auditSuccess = (bool)(await cmd.ExecuteScalarAsync())!;
        await Assert.That(auditSuccess).IsTrue();
    }

    [Test]
    [Skip("Requires PostgreSQL test container - enable for integration tests")]
    public async Task CreateTenantAndUserAsync_DuplicateEmail_ThrowsException()
    {
        // Arrange - create first user
        SignupRequest firstRequest = new()
        {
            TenantId = PrefixedUlid.Generate("tenant"),
            TenantName = "First User",
            UserId = PrefixedUlid.Generate("user"),
            FirstName = "First",
            LastName = "User",
            Email = "duplicate@example.com",
            ExternalAuthId = "logto-first"
        };
        
        await _signupService!.CreateTenantAndUserAsync(firstRequest);

        // Arrange - try to create second user with same email
        SignupRequest duplicateRequest = new()
        {
            TenantId = PrefixedUlid.Generate("tenant"),
            TenantName = "Second User",
            UserId = PrefixedUlid.Generate("user"),
            FirstName = "Second",
            LastName = "User",
            Email = "duplicate@example.com", // Same email
            ExternalAuthId = "logto-second"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _signupService.CreateTenantAndUserAsync(duplicateRequest));
        
        // Verify audit log shows failure
        await using DbContext context = await _contextFactory.CreateDbContextAsync();
        NpgsqlConnection? connection = context.Database.GetDbConnection() as NpgsqlConnection;
        await connection!.OpenAsync();
        
        await using NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT ""Success"", ""ErrorMessage"" 
            FROM ""SignupAuditLog"" 
            WHERE ""Email"" = @email 
            ORDER BY ""CreatedAt"" DESC 
            LIMIT 1";
        cmd.Parameters.AddWithValue("@email", duplicateRequest.Email);
        
        await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        
        bool success = reader.GetBoolean(0);
        string? errorMessage = reader.IsDBNull(1) ? null : reader.GetString(1);
        
        await Assert.That(success).IsFalse();
        await Assert.That(errorMessage).Contains("Email already");
    }

    [Test]
    [Skip("Requires PostgreSQL test container - enable for integration tests")]
    public async Task CreateTenantAndUserAsync_InvalidTenantIdFormat_ThrowsException()
    {
        // Arrange
        SignupRequest request = new()
        {
            TenantId = "invalid-id-format", // Invalid format
            TenantName = "Test User",
            UserId = PrefixedUlid.Generate("user"),
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            ExternalAuthId = "logto-12345"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _signupService!.CreateTenantAndUserAsync(request));
    }

    [Test]
    [Skip("Requires PostgreSQL test container - enable for integration tests")]
    public async Task CreateTenantAndUserAsync_InvalidEmail_ThrowsException()
    {
        // Arrange
        SignupRequest request = new()
        {
            TenantId = PrefixedUlid.Generate("tenant"),
            TenantName = "Test User",
            UserId = PrefixedUlid.Generate("user"),
            FirstName = "Test",
            LastName = "User",
            Email = "not-an-email", // Invalid email format
            ExternalAuthId = "logto-12345"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _signupService!.CreateTenantAndUserAsync(request));
    }

    [Test]
    [Skip("Requires PostgreSQL test container - enable for integration tests")]
    public async Task CreateTenantAndUserAsync_RateLimit_ThrowsAfterFiveAttempts()
    {
        // Arrange - create 5 failed signups (same email)
        string testEmail = "ratelimit@example.com";
        
        for (int i = 0; i < 5; i++)
        {
            try
            {
                // Use invalid tenant ID to force failure
                await _signupService!.CreateTenantAndUserAsync(new SignupRequest
                {
                    TenantId = "invalid",
                    TenantName = "Test",
                    UserId = PrefixedUlid.Generate("user"),
                    FirstName = "Test",
                    LastName = "User",
                    Email = testEmail,
                    ExternalAuthId = $"logto-{i}"
                });
            }
            catch
            {
                // Expected to fail
            }
        }

        // Act - 6th attempt should be rate limited
        SignupRequest sixthRequest = new()
        {
            TenantId = "invalid",
            TenantName = "Test User",
            UserId = PrefixedUlid.Generate("user"),
            FirstName = "Test",
            LastName = "User",
            Email = testEmail,
            ExternalAuthId = "logto-sixth"
        };

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _signupService!.CreateTenantAndUserAsync(sixthRequest));
        
        await Assert.That(exception.Message).Contains("Rate limit");
    }

    [Test]
    [Skip("Requires PostgreSQL test container - enable for integration tests")]
    public async Task CreateTenantAndUserAsync_UserAndTenantLinkedCorrectly()
    {
        // Arrange
        SignupRequest request = new()
        {
            TenantId = PrefixedUlid.Generate("tenant"),
            TenantName = "Test User",
            UserId = PrefixedUlid.Generate("user"),
            FirstName = "Test",
            LastName = "User",
            Email = "linked@example.com",
            ExternalAuthId = "logto-12345"
        };

        // Act
        await _signupService!.CreateTenantAndUserAsync(request);

        // Assert - Verify user is linked to correct tenant
        await using DbContext context = await _contextFactory.CreateDbContextAsync();
        NpgsqlConnection? connection = context.Database.GetDbConnection() as NpgsqlConnection;
        await connection!.OpenAsync();
        
        await using NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT \"TenantId\" FROM \"Users\" WHERE \"Id\" = @id";
        cmd.Parameters.AddWithValue("@id", request.UserId);
        
        string? tenantId = (await cmd.ExecuteScalarAsync()) as string;
        await Assert.That(tenantId).IsEqualTo(request.TenantId);
    }

    [Test]
    [Skip("Requires PostgreSQL test container - enable for integration tests")]
    public async Task CreateTenantAndUserAsync_AuditLogContainsIPAndUserAgent()
    {
        // Arrange
        SignupRequest request = new()
        {
            TenantId = PrefixedUlid.Generate("tenant"),
            TenantName = "Test User",
            UserId = PrefixedUlid.Generate("user"),
            FirstName = "Test",
            LastName = "User",
            Email = "audit@example.com",
            ExternalAuthId = "logto-12345",
            IpAddress = "192.168.1.100",
            UserAgent = "Mozilla/5.0 (Test Browser)"
        };

        // Act
        await _signupService!.CreateTenantAndUserAsync(request);

        // Assert - Verify audit log contains IP and user agent
        await using DbContext context = await _contextFactory.CreateDbContextAsync();
        NpgsqlConnection? connection = context.Database.GetDbConnection() as NpgsqlConnection;
        await connection!.OpenAsync();
        
        await using NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT ""IpAddress"", ""UserAgent"" 
            FROM ""SignupAuditLog"" 
            WHERE ""Email"" = @email 
            ORDER BY ""CreatedAt"" DESC 
            LIMIT 1";
        cmd.Parameters.AddWithValue("@email", request.Email);
        
        await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        
        string? ipAddress = reader.IsDBNull(0) ? null : reader.GetString(0);
        string? userAgent = reader.IsDBNull(1) ? null : reader.GetString(1);
        
        await Assert.That(ipAddress).IsEqualTo(request.IpAddress);
        await Assert.That(userAgent).IsEqualTo(request.UserAgent);
    }

    public async ValueTask DisposeAsync()
    {
        // Cleanup resources
        await Task.CompletedTask;
    }
}
