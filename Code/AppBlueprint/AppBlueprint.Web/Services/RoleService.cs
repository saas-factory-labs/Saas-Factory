using System.Net.Http.Json;
using System.Text.Json;
using AppBlueprint.Contracts.Baseline.Role.Requests;
using AppBlueprint.Contracts.Baseline.Role.Responses;

namespace AppBlueprint.Web.Services;

/// <summary>
/// Service for managing roles via API calls
/// </summary>
public class RoleService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RoleService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RoleService(
        HttpClient httpClient,
        ILogger<RoleService> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(logger);

        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Gets all roles
    /// </summary>
    public async Task<IEnumerable<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(new Uri("/api/v1/roles", UriKind.Relative), cancellationToken);

            // If error, read response body for details before throwing
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Roles API error {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
                throw new HttpRequestException($"API returned {response.StatusCode}: {errorContent}");
            }

            var roles = await response.Content.ReadFromJsonAsync<IEnumerable<RoleResponse>>(_jsonOptions, cancellationToken);
            return roles ?? Enumerable.Empty<RoleResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching roles");
            throw;
        }
    }

    /// <summary>
    /// Gets a specific role by ID
    /// </summary>
    public async Task<RoleResponse?> GetRoleByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        try
        {
            var response = await _httpClient.GetAsync(new Uri($"/api/v1/roles/{id}", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<RoleResponse>(_jsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching role {RoleId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new role
    /// </summary>
    public async Task<RoleResponse?> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/roles", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<RoleResponse>(_jsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing role
    /// </summary>
    public async Task UpdateRoleAsync(string id, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/v1/roles/{id}", request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a role
    /// </summary>
    public async Task DeleteRoleAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        try
        {
            var response = await _httpClient.DeleteAsync(new Uri($"/api/v1/roles/{id}", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            throw;
        }
    }
}
