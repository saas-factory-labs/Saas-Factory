using AppBlueprint.Contracts.Baseline.Subscription.Requests;
using AppBlueprint.Contracts.Baseline.Subscription.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/subscriptions")]
[Produces("application/json")]
public class SubscriptionController : BaseController
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionController(
        IConfiguration configuration,
        ISubscriptionRepository subscriptionRepository)
        : base(configuration)
    {
        _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
    }

    /// <summary>
    /// Gets all subscriptions.
    /// </summary>
    /// <returns>List of subscriptions</returns>
    [HttpGet(ApiEndpoints.Subscriptions.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<IEnumerable<SubscriptionResponse>>> GetSubscriptions(CancellationToken cancellationToken)
    {
        IEnumerable<SubscriptionEntity> subscriptions = await _subscriptionRepository.GetAllAsync(cancellationToken);
        if (!subscriptions.Any())
            return NotFound(new { Message = "No subscriptions found." });

        IEnumerable<SubscriptionResponse> response = subscriptions.Select(s => new SubscriptionResponse
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Code = s.Code,
            Status = s.Status,
            CreatedAt = s.CreatedAt
        });

        return Ok(response);
    }

    /// <summary>
    /// Gets a subscription by ID.
    /// </summary>
    /// <param name="id">Subscription ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Subscription</returns>
    [HttpGet(ApiEndpoints.Subscriptions.GetById)]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<SubscriptionResponse>> GetSubscription(string id, CancellationToken cancellationToken)
    {
        SubscriptionEntity? subscription = await _subscriptionRepository.GetByIdAsync(id, cancellationToken);
        if (subscription is null)
            return NotFound(new { Message = $"Subscription with ID {id} not found." });

        var response = new SubscriptionResponse
        {
            Id = subscription.Id,
            Name = subscription.Name,
            Description = subscription.Description,
            Code = subscription.Code,
            Status = subscription.Status,
            CreatedAt = subscription.CreatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a new subscription.
    /// </summary>
    /// <param name="dto">Subscription data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created subscription.</returns>
    [HttpPost(ApiEndpoints.Subscriptions.Create)]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<SubscriptionResponse>> CreateSubscription(
        [FromBody] CreateSubscriptionRequest dto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        string userId = User.FindFirst("sub")?.Value ?? "unknown";

        var entity = new SubscriptionEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            Code = dto.Code,
            Status = dto.Status,
            CreatedBy = userId,
            UpdatedBy = userId,
            TenantId = HttpContext.Items["TenantId"]?.ToString() ?? string.Empty
        };

        await _subscriptionRepository.AddAsync(entity, cancellationToken);

        var response = new SubscriptionResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Code = entity.Code,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt
        };

        return CreatedAtAction(nameof(GetSubscription), new { id = entity.Id }, response);
    }

    /// <summary>
    /// Updates an existing subscription.
    /// </summary>
    /// <param name="id">Subscription ID.</param>
    /// <param name="dto">Updated subscription data.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpPut(ApiEndpoints.Subscriptions.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> UpdateSubscription(
        string id,
        [FromBody] UpdateSubscriptionRequest dto,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        SubscriptionEntity? existing = await _subscriptionRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(new { Message = $"Subscription with ID {id} not found." });

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.Status = dto.Status;
        existing.UpdatedBy = User.FindFirst("sub")?.Value ?? "unknown";
        existing.LastUpdatedAt = DateTime.UtcNow;

        await _subscriptionRepository.UpdateAsync(existing, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Deletes a subscription by ID.
    /// </summary>
    /// <param name="id">Subscription ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpDelete(ApiEndpoints.Subscriptions.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult> DeleteSubscription(string id, CancellationToken cancellationToken)
    {
        SubscriptionEntity? existing = await _subscriptionRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(new { Message = $"Subscription with ID {id} not found." });

        await _subscriptionRepository.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}

