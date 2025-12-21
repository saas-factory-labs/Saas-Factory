using System.Net.Http.Json;
using System.Text.Json;
using AppBlueprint.TodoAppKernel.Controllers;
using AppBlueprint.TodoAppKernel.Domain;
using AppBlueprint.Infrastructure.Authorization;
using Microsoft.JSInterop;

namespace AppBlueprint.Web.Services;

/// <summary>
/// Service for managing todo items via API calls
/// </summary>
public class TodoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TodoService> _logger;
    private readonly ITokenStorageService _tokenStorageService;
    private readonly JsonSerializerOptions _jsonOptions;

    public TodoService(
        HttpClient httpClient, 
        ILogger<TodoService> logger,
        ITokenStorageService tokenStorageService)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(tokenStorageService);

        _httpClient = httpClient;
        _logger = logger;
        _tokenStorageService = tokenStorageService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Add authentication and tenant headers to HttpRequestMessage
    /// </summary>
    private async Task<bool> AddAuthHeadersAsync(HttpRequestMessage request)
    {
        _logger.LogInformation("=== AddAuthHeadersAsync CALLED ===");
        
        try
        {
            _logger.LogInformation("Attempting to get token from storage...");
            
            // Get token from storage
            var token = await _tokenStorageService.GetTokenAsync();
            
            _logger.LogInformation("Token retrieval complete. Has token: {HasToken}, Token length: {Length}", 
                !string.IsNullOrEmpty(token), 
                token?.Length ?? 0);
            
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                _logger.LogInformation("✅ Added authorization header to request. Token preview: {Preview}", 
                    string.Concat(token.AsSpan(0, Math.Min(20, token.Length)), "..."));
            }
            else
            {
                _logger.LogWarning("❌ No token found in storage");
                return false;
            }

            // Add tenant-id header
            _logger.LogInformation("Getting tenant ID...");
            var tenantId = await GetTenantIdAsync();
            request.Headers.Add("tenant-id", tenantId);
            _logger.LogInformation("✅ Added tenant-id header: {TenantId}", tenantId);

            _logger.LogInformation("=== AddAuthHeadersAsync SUCCESS ===");
            return true;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop", StringComparison.Ordinal))
        {
            _logger.LogWarning("❌ JavaScript interop not available (prerendering): {Message}", ex.Message);
            return false;
        }
        catch (JSException ex)
        {
            _logger.LogWarning("❌ JavaScript exception: {Message}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ UNEXPECTED ERROR adding auth headers: {Type} - {Message}", 
                ex.GetType().Name, 
                ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Get tenant ID from storage or use default
    /// </summary>
    private async Task<string> GetTenantIdAsync()
    {
        try
        {
            var tenantId = await _tokenStorageService.GetValueAsync("tenant_id");
            if (!string.IsNullOrEmpty(tenantId))
            {
                return tenantId;
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Could not retrieve tenant ID: {Message}", ex.Message);
        }

        return "default-tenant";
    }

    /// <summary>
    /// Test API connectivity (no authentication required)
    /// </summary>
    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Testing connection to API at {BaseAddress}", _httpClient.BaseAddress);
            var response = await _httpClient.GetAsync(new Uri("/api/AuthDebug/ping", UriKind.Relative), cancellationToken);
            
            _logger.LogInformation(
                "Connection test result: Status={StatusCode}, Success={IsSuccess}",
                response.StatusCode,
                response.IsSuccessStatusCode);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Connection test failed: {Content}", content);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request exception during connection test: {Message}", ex.Message);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Connection test timed out");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error testing API connection");
            return false;
        }
    }

    /// <summary>
    /// Test authenticated API connectivity
    /// </summary>
    public async Task<string> TestAuthenticatedConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Testing authenticated connection to API");
            
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/AuthDebug/secure-ping");
            var headersAdded = await AddAuthHeadersAsync(request);
            
            if (!headersAdded)
            {
                return "❌ No authentication token available (not logged in or JavaScript not ready)";
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogInformation(
                "Authenticated test response: Status={Status}, Content={Content}",
                response.StatusCode,
                content);
            
            if (response.IsSuccessStatusCode)
            {
                return $"✅ Status: {response.StatusCode} - Authentication successful!";
            }
            else
            {
                return $"❌ Status: {response.StatusCode} - {content}";
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request exception during auth test");
            return $"❌ HTTP Error: {ex.Message}";
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Auth test timed out");
            return "❌ Request timed out";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error testing authenticated connection");
            return $"❌ Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Get diagnostic information about what's being sent to API
    /// </summary>
    public async Task<string> GetDiagnosticInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("=== GetDiagnosticInfoAsync CALLED ===");
            
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/AuthDebug/headers");
            
            _logger.LogInformation("Calling AddAuthHeadersAsync...");
            var headersAdded = await AddAuthHeadersAsync(request);
            _logger.LogInformation("AddAuthHeadersAsync returned: {Result}", headersAdded);
            
            var response = await _httpClient.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogInformation("Diagnostic info received: {Content}", content);
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting diagnostic info");
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Gets all todos for the current user
    /// </summary>
    public async Task<IEnumerable<TodoEntity>> GetTodosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1.0/todo");
            await AddAuthHeadersAsync(request);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var todos = await response.Content.ReadFromJsonAsync<IEnumerable<TodoEntity>>(_jsonOptions, cancellationToken);
            return todos ?? Enumerable.Empty<TodoEntity>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching todos");
            throw;
        }
    }

    /// <summary>
    /// Creates a new todo item
    /// </summary>
    public async Task<TodoEntity?> CreateTodoAsync(CreateTodoRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/todo")
            {
                Content = JsonContent.Create(request)
            };
            await AddAuthHeadersAsync(httpRequest);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TodoEntity>(_jsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo");
            throw;
        }
    }

    /// <summary>
    /// Gets a specific todo by ID
    /// </summary>
    public async Task<TodoEntity?> GetTodoByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        try
        {
            var response = await _httpClient.GetAsync(new Uri($"/api/v1.0/todo/{id}", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TodoEntity>(_jsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching todo {TodoId}", id);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing todo item
    /// </summary>
    public async Task<TodoEntity?> UpdateTodoAsync(string id, UpdateTodoRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/v1.0/todo/{id}", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TodoEntity>(_jsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo {TodoId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a todo item
    /// </summary>
    public async Task DeleteTodoAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        try
        {
            var response = await _httpClient.DeleteAsync(new Uri($"/api/v1.0/todo/{id}", UriKind.Relative), cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo {TodoId}", id);
            throw;
        }
    }

    /// <summary>
    /// Marks a todo as completed
    /// </summary>
    public async Task<TodoEntity?> CompleteTodoAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        try
        {
            var response = await _httpClient.PatchAsync(new Uri($"/api/v1.0/todo/{id}/complete", UriKind.Relative), null, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TodoEntity>(_jsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing todo {TodoId}", id);
            throw;
        }
    }
}

