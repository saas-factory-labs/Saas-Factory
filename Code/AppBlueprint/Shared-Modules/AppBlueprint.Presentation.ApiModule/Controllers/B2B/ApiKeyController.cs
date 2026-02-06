using AppBlueprint.Application.Constants;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Contracts.B2B.Contracts.ApiKey.Requests;
using AppBlueprint.Contracts.B2B.Contracts.ApiKey.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.Profile;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.B2B;

[Authorize(Roles = Roles.TenantAdmin)]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/api-key")]
[Produces("application/json")]
public class ApiKeyController : BaseController
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IConfiguration _configuration;
    // Removed IUnitOfWork dependency for repository DI pattern

    public ApiKeyController(IConfiguration configuration, IApiKeyRepository apiKeyRepository) :
        base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _apiKeyRepository = apiKeyRepository ?? throw new ArgumentNullException(nameof(apiKeyRepository));
        // Removed IUnitOfWork assignment
    }

    /// <summary>
    ///     Gets all API keys.
    /// </summary>
    /// <returns>List of API keys</returns>
    [HttpGet("GetApiKeys")]
    [ProducesResponseType(typeof(IEnumerable<ApiKeyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<IEnumerable<ApiKeyResponse>>> GetApiKeys(CancellationToken cancellationToken)
    {
        IEnumerable<ApiKeyEntity>? apiKeys = await _apiKeyRepository.GetAllAsync();
        if (!apiKeys.Any()) return NotFound(new { Message = "No API keys found." });

        IEnumerable<ApiKeyResponse>? response = apiKeys.Select(apiKey => new ApiKeyResponse
        {
            Name = apiKey.Name
        });

        return Ok(response);
    }

    /// <summary>
    ///     Gets an API key by ID.
    /// </summary>
    /// <param name="id">API key ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>    /// <returns>API key</returns>
    [HttpGet("GetApiKey/{id}")]
    [ProducesResponseType(typeof(ApiKeyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<ApiKeyResponse>> GetApiKey(string id, CancellationToken cancellationToken)
    {
        ApiKeyEntity? apiKey = await _apiKeyRepository.GetByIdAsync(id);
        if (apiKey is null) return NotFound(new { Message = $"API key with ID {id} not found." });

        var response = new ApiKeyResponse
        {
            Name = apiKey.Name
            // Email = apiKey.Email
        };

        return Ok(response);
    }

    /// <summary>
    ///     Creates a new API key.
    /// </summary>
    /// <param name="apiKeyDto">API key data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created API key.</returns>
    [HttpPost(ApiEndpoints.ApiKeys.Create)]
    [ProducesResponseType(typeof(ApiKeyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> CreateApiKey([FromBody] CreateApiKeyRequest apiKeyDto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(apiKeyDto);

        if (!ModelState.IsValid) return BadRequest(ModelState);

        string tenantId = HttpContext.Items["TenantId"]?.ToString() ?? "default-tenant";
        string userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value ?? "unknown-user";
        string? userEmail = User.FindFirst("email")?.Value;

        var newApiKey = new ApiKeyEntity
        {
            Name = apiKeyDto.Name,
            SecretRef = "adad",
            TenantId = tenantId,
            Owner = new UserEntity
            {
                Email = userEmail ?? "unknown@example.com",
                FirstName = User.FindFirst("given_name")?.Value ?? "Unknown",
                LastName = User.FindFirst("family_name")?.Value ?? "User",
                UserName = User.Identity?.Name ?? userId,
                Profile = new ProfileEntity()
            }
        };

        await _apiKeyRepository.AddAsync(newApiKey);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return CreatedAtAction(nameof(GetApiKey), new { id = newApiKey.Id }, newApiKey);
    }

    /// <summary>
    ///     Updates an existing API key.
    /// </summary>
    /// <param name="id">API key ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <param name="apiKeyDto">API key data transfer object.</param>
    /// <returns>No content.</returns>
    [HttpPut(ApiEndpoints.ApiKeys.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> UpdateApiKey(string id, [FromBody] UpdateApiKeyRequest apiKeyDto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(apiKeyDto);
        ApiKeyEntity? existingApiKey = await _apiKeyRepository.GetByIdAsync(id);
        if (existingApiKey is null) return NotFound(new { Message = $"API key with ID {id} not found." });

        existingApiKey.Name = apiKeyDto.Name;

        _apiKeyRepository.Update(existingApiKey);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes an API key by ID.
    /// </summary>
    /// <param name="id">API key ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>    [HttpDelete(ApiEndpoints.ApiKeys.DeleteById)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> DeleteApiKey(string id, CancellationToken cancellationToken)
    {
        ApiKeyEntity? existingApiKey = await _apiKeyRepository.GetByIdAsync(id);
        if (existingApiKey is null) return NotFound(new { Message = $"API key with ID {id} not found." });

        _apiKeyRepository.Delete(existingApiKey.Id);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }
}
