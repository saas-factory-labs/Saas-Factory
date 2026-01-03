using System.Data.Common;
using System.Text.Json;
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
/// Database connection provider for signup operations.
/// Abstraction to allow Application layer to access database without depending on Infrastructure.
/// </summary>
public interface ISignupDbConnectionProvider
{
    /// <summary>
    /// Gets a database connection for signup operations.
    /// </summary>
    Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
}

public sealed class SignupService : ISignupService
{
    private readonly ISignupDbConnectionProvider _connectionProvider;
    private readonly ILogger<SignupService> _logger;

    public SignupService(
        ISignupDbConnectionProvider connectionProvider,
        ILogger<SignupService> logger)
    {
        ArgumentNullException.ThrowIfNull(connectionProvider);
        ArgumentNullException.ThrowIfNull(logger);

        _connectionProvider = connectionProvider;
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
            // Get database connection
            DbConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);
            
            if (connection is not NpgsqlConnection npgsqlConnection)
            {
                throw new InvalidOperationException("Database connection is not PostgreSQL (Npgsql)");
            }

            // Ensure connection is open
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            // Call stored procedure with parameterized query (prevents SQL injection)
            await using NpgsqlCommand command = npgsqlConnection.CreateCommand();
            command.CommandText = @"
                SELECT create_tenant_and_user(
                    @p_tenant_id,
                    @p_tenant_name,
                    @p_user_id,
                    @p_user_first_name,
                    @p_user_last_name,
                    @p_user_email,
                    @p_external_auth_id,
                    @p_ip_address,
                    @p_user_agent
                );";

            // Add parameters (all properly typed to prevent injection)
            command.Parameters.AddWithValue("@p_tenant_id", request.TenantId);
            command.Parameters.AddWithValue("@p_tenant_name", request.TenantName);
            command.Parameters.AddWithValue("@p_user_id", request.UserId);
            command.Parameters.AddWithValue("@p_user_first_name", request.FirstName);
            command.Parameters.AddWithValue("@p_user_last_name", request.LastName);
            command.Parameters.AddWithValue("@p_user_email", request.Email);
            command.Parameters.AddWithValue("@p_external_auth_id", request.ExternalAuthId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@p_ip_address", request.IpAddress ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@p_user_agent", request.UserAgent ?? (object)DBNull.Value);

            // Execute and read JSON result
            object? result = await command.ExecuteScalarAsync(cancellationToken);
            
            if (result is null)
            {
                throw new InvalidOperationException("Stored procedure returned null");
            }

            string jsonResult = result.ToString() ?? throw new InvalidOperationException("Result is not valid JSON");

            // Parse JSON response
            JsonDocument jsonDoc = JsonDocument.Parse(jsonResult);
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
