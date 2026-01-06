using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AppBlueprint.Application.Services;

/// <summary>
/// Service for handling secure signup operations via database stored procedure.
/// Uses SECURITY DEFINER stored procedure to bypass RLS during initial account creation.
/// </summary>
public interface ISignupService
{
    /// <summary>
    /// Creates a new tenant and user atomically via secure stored procedure.
    /// </summary>
    Task<SignupResult> CreateTenantAndUserAsync(
        SignupRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Database context provider for signup operations.
/// Provides access to DbContext for EF Core operations.
/// </summary>
public interface ISignupDbContextProvider
{
    /// <summary>
    /// Gets a database context for signup operations.
    /// </summary>
    Task<DbContext> GetDbContextAsync(CancellationToken cancellationToken = default);
}

public sealed class SignupService : ISignupService
{
    private readonly ISignupDbContextProvider _contextProvider;
    private readonly ILogger<SignupService> _logger;

    public SignupService(
        ISignupDbContextProvider contextProvider,
        ILogger<SignupService> logger)
    {
        ArgumentNullException.ThrowIfNull(contextProvider);
        ArgumentNullException.ThrowIfNull(logger);

        _contextProvider = contextProvider;
        _logger = logger;
    }

    public async Task<SignupResult> CreateTenantAndUserAsync(
        SignupRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Calling secure signup stored procedure for email: {Email}",
            request.Email);

        try
        {
            // Get EF Core DbContext (properly manages connection lifecycle)
            DbContext context = await _contextProvider.GetDbContextAsync(cancellationToken);
            
            // Use EF Core's ExecuteSqlRawAsync to call stored procedure
            // The stored procedure returns JSON, which we'll query using FromSqlRaw
            FormattableString sql = $@"
                SELECT create_tenant_and_user(
                    {request.TenantId},
                    {request.TenantName},
                    {request.TenantType},
                    {request.UserId},
                    {request.FirstName},
                    {request.LastName},
                    {request.Email},
                    {request.ExternalAuthId},
                    {request.IpAddress},
                    {request.UserAgent}
                ) AS ""JsonResult""";
            
            // Execute the stored procedure and get the JSON result
            var results = await context.Database
                .SqlQuery<SignupStoredProcedureResult>(sql)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (results.Count == 0 || results[0] is null)
            {
                throw new InvalidOperationException("Stored procedure returned no results");
            }

            // Parse the JSON result from the stored procedure
            JsonDocument jsonDoc = JsonDocument.Parse(results[0].JsonResult);
            JsonElement root = jsonDoc.RootElement;

            bool success = root.GetProperty("success").GetBoolean();
            if (!success)
            {
                string? errorMessage = root.TryGetProperty("error_message", out JsonElement errorElement)
                    ? errorElement.GetString()
                    : "Unknown error";

                _logger.LogError(
                    "Signup stored procedure failed for email {Email}: {Error}",
                    request.Email,
                    errorMessage);

                throw new InvalidOperationException($"Signup failed: {errorMessage}");
            }

            var signupResult = new SignupResult
            {
                Success = true,
                TenantId = root.GetProperty("tenant_id").GetString() ?? throw new InvalidOperationException("Missing tenant_id"),
                UserId = root.GetProperty("user_id").GetString() ?? throw new InvalidOperationException("Missing user_id"),
                ProfileId = root.GetProperty("profile_id").GetString() ?? throw new InvalidOperationException("Missing profile_id"),
                Email = root.GetProperty("email").GetString() ?? throw new InvalidOperationException("Missing email"),
                CreatedAt = root.GetProperty("created_at").GetDateTime()
            };

            _logger.LogInformation(
                "Signup successful - TenantId: {TenantId}, UserId: {UserId}, Email: {Email}",
                signupResult.TenantId,
                signupResult.UserId,
                signupResult.Email);

            return signupResult;
        }
        catch (PostgresException pgEx)
        {
            // PostgreSQL exception (validation errors from stored procedure)
            _logger.LogError(
                pgEx,
                "PostgreSQL error during signup for email {Email}: {Message}",
                request.Email,
                pgEx.MessageText);

            throw new InvalidOperationException($"Signup failed: {pgEx.MessageText}", pgEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error during signup for email {Email}",
                request.Email);

            throw;
        }
    }
}

/// <summary>
/// Internal result type for mapping stored procedure JSON output.
/// The stored procedure returns a single JSON column.
/// </summary>
internal sealed class SignupStoredProcedureResult
{
    /// <summary>
    /// The JSON result returned by create_tenant_and_user stored procedure.
    /// Maps to the single column returned by the function.
    /// </summary>
    public string JsonResult { get; init; } = string.Empty;
}

/// <summary>
/// Request model for secure signup operation.
/// </summary>
public sealed record SignupRequest
{
    public required string TenantId { get; init; }
    public required string TenantName { get; init; }
    public required string UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public string? ExternalAuthId { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    /// <summary>
    /// Tenant type: 0 = Personal (B2C), 1 = Organization (B2B).
    /// </summary>
    public required int TenantType { get; init; }
}

/// <summary>
/// Result model for signup operation.
/// </summary>
public sealed record SignupResult
{
    public required bool Success { get; init; }
    public required string TenantId { get; init; }
    public required string UserId { get; init; }
    public required string ProfileId { get; init; }
    public required string Email { get; init; }
    public required DateTime CreatedAt { get; init; }
}
