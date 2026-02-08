using System.Text.Json;
using AppBlueprint.Contracts.B2B.Contracts.Team.Requests;
using AppBlueprint.Contracts.B2B.Contracts.Team.Responses;

namespace AppBlueprint.Web.Services;

/// <summary>
/// Service for managing teams via API calls
/// </summary>
internal sealed class TeamService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TeamService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public TeamService(
        HttpClient httpClient,
        ILogger<TeamService> logger)
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
    /// Gets all teams
    /// </summary>
    public async Task<IEnumerable<TeamResponse>> GetTeamsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(new Uri("/api/v1/teams", UriKind.Relative), cancellationToken);

            // If error, read response body for details before throwing
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Teams API error {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
                throw new HttpRequestException($"API returned {response.StatusCode}: {errorContent}");
            }

            var teams = await response.Content.ReadFromJsonAsync<IEnumerable<TeamResponse>>(_jsonOptions, cancellationToken);
            return teams ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching teams");
            throw;
        }
    }

    /// <summary>
    /// Gets a specific team by ID
    /// </summary>
    public async Task<TeamResponse?> GetTeamByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        try
        {
            var response = await _httpClient.GetAsync(new Uri($"/api/v1/teams/{id}", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TeamResponse>(_jsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching team {TeamId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new team
    /// </summary>
    public async Task<TeamResponse?> CreateTeamAsync(CreateTeamRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/teams", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TeamResponse>(_jsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing team
    /// </summary>
    public async Task UpdateTeamAsync(string id, UpdateTeamRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/v1/teams/{id}", request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team {TeamId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a team
    /// </summary>
    public async Task DeleteTeamAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        try
        {
            var response = await _httpClient.DeleteAsync(new Uri($"/api/v1/teams/{id}", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting team {TeamId}", id);
            throw;
        }
    }
}
