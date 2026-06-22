using System.Text.Json;
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Signup;
using AppBlueprint.Application.Signup.Internal;
using AppBlueprint.SharedKernel;
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
        AuthenticatedSignupIdentity authenticatedIdentity,
        CancellationToken cancellationToken = default);
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
        AuthenticatedSignupIdentity authenticatedIdentity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(authenticatedIdentity);

        ValidatedSignupRequest validatedRequest = ValidateAndNormalizeRequest(request, authenticatedIdentity);

        _logger.LogInformation(
            "Calling secure signup stored procedure for UserId {UserId} and TenantId {TenantId}",
            validatedRequest.UserId,
            validatedRequest.TenantId);

        try
        {
            // Get EF Core DbContext (properly manages connection lifecycle)
            DbContext context = await _contextProvider.GetDbContextAsync(cancellationToken);

            // Use EF Core's ExecuteSqlRawAsync to call stored procedure
            // The stored procedure returns JSON, which we'll query using FromSqlRaw
            FormattableString sql = $@"
                SELECT create_tenant_and_user(
                    {validatedRequest.TenantId},
                    {validatedRequest.TenantName},
                    {(int)validatedRequest.TenantType},
                    {validatedRequest.UserId},
                    {validatedRequest.FirstName},
                    {validatedRequest.LastName},
                    {validatedRequest.Email},
                    {validatedRequest.ExternalAuthId},
                    {validatedRequest.IpAddress},
                    {validatedRequest.UserAgent}
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
            var jsonDoc = JsonDocument.Parse(results[0].JsonResult);
            JsonElement root = jsonDoc.RootElement;

            bool success = root.GetProperty("success").GetBoolean();
            if (!success)
            {
                string? errorMessage = root.TryGetProperty("error_message", out JsonElement errorElement)
                    ? errorElement.GetString()
                    : "Unknown error";

                _logger.LogError(
                    "Signup stored procedure failed for UserId {UserId} and TenantId {TenantId}: {Error}",
                    validatedRequest.UserId,
                    validatedRequest.TenantId,
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
                "Signup successful - TenantId: {TenantId}, UserId: {UserId}",
                signupResult.TenantId,
                signupResult.UserId);

            return signupResult;
        }
        catch (PostgresException pgEx)
        {
            // PostgreSQL exception (validation errors from stored procedure)
            _logger.LogError(
                pgEx,
                "PostgreSQL error during signup for UserId {UserId} and TenantId {TenantId}: {Message}",
                validatedRequest.UserId,
                validatedRequest.TenantId,
                pgEx.MessageText);

            throw new InvalidOperationException($"Signup failed: {pgEx.MessageText}", pgEx);
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(
                ex,
                "Database error during signup for UserId {UserId} and TenantId {TenantId}",
                validatedRequest.UserId,
                validatedRequest.TenantId);

            throw new InvalidOperationException("Signup failed due to a database error.", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Failed to parse signup result for UserId {UserId} and TenantId {TenantId}",
                validatedRequest.UserId,
                validatedRequest.TenantId);

            throw new InvalidOperationException("Signup failed due to an invalid response format.", ex);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error during signup for UserId {UserId} and TenantId {TenantId}",
                validatedRequest.UserId,
                validatedRequest.TenantId);

            throw;
        }
    }

    /// <summary>
    /// Validates untrusted signup input and combines it with trusted authenticated identity.
    /// </summary>
    private static ValidatedSignupRequest ValidateAndNormalizeRequest(
        SignupRequest request,
        AuthenticatedSignupIdentity authenticatedIdentity)
    {
        string firstName = NormalizeRequiredValue(
            request.FirstName,
            maxLength: 50,
            errorMessage: "First name is required.",
            parameterName: nameof(request.FirstName));
        string lastName = NormalizeRequiredValue(
            request.LastName,
            maxLength: 50,
            errorMessage: "Last name is required.",
            parameterName: nameof(request.LastName));
        string email = NormalizeRequiredValue(
            authenticatedIdentity.Email,
            maxLength: 320,
            errorMessage: "Email is required.",
            parameterName: nameof(authenticatedIdentity.Email));
        string externalAuthId = NormalizeRequiredValue(
            authenticatedIdentity.ExternalAuthId,
            maxLength: 200,
            errorMessage: "External auth ID is required.",
            parameterName: nameof(authenticatedIdentity.ExternalAuthId));

        if (!request.AcceptTerms)
        {
            throw new ArgumentException("Terms and conditions must be accepted.", nameof(request));
        }

        if (!Enum.IsDefined(request.TenantType))
        {
            throw new ArgumentException("Tenant type is invalid.", nameof(request));
        }

        string tenantName = request.TenantType switch
        {
            SignupTenantType.Personal => $"{firstName} {lastName}",
            SignupTenantType.Organization => NormalizeOrganizationTenantName(request.CompanyName, request.Country),
            _ => throw new ArgumentException("Tenant type is invalid.", nameof(request))
        };

        return new ValidatedSignupRequest
        {
            TenantId = PrefixedUlid.Generate("tenant"),
            TenantName = tenantName,
            UserId = PrefixedUlid.Generate("user"),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            ExternalAuthId = externalAuthId,
            IpAddress = NormalizeOptionalValue(request.IpAddress, maxLength: 128),
            UserAgent = NormalizeOptionalValue(request.UserAgent, maxLength: 1024),
            TenantType = request.TenantType
        };
    }

    /// <summary>
    /// Validates the organization-specific fields required for a business signup.
    /// </summary>
    private static string NormalizeOrganizationTenantName(string? companyName, string? country)
    {
        _ = NormalizeRequiredValue(
            country,
            maxLength: 100,
            errorMessage: "Country is required for organization signups.",
            parameterName: nameof(SignupRequest.Country));

        return NormalizeRequiredValue(
            companyName,
            maxLength: 200,
            errorMessage: "Company name is required for organization signups.",
            parameterName: nameof(SignupRequest.CompanyName));
    }

    /// <summary>
    /// Normalizes a required input value and enforces a maximum length.
    /// </summary>
    private static string NormalizeRequiredValue(
        string? value,
        int maxLength,
        string errorMessage,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(errorMessage, parameterName);
        }

        string normalizedValue = value.Trim();

        if (normalizedValue.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} cannot exceed {maxLength} characters.", parameterName);
        }

        return normalizedValue;
    }

    /// <summary>
    /// Normalizes an optional value and enforces a maximum length when present.
    /// </summary>
    private static string? NormalizeOptionalValue(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        string normalizedValue = value.Trim();

        if (normalizedValue.Length > maxLength)
        {
            throw new ArgumentException($"Optional value cannot exceed {maxLength} characters.");
        }

        return normalizedValue;
    }
}
