using AppBlueprint.Application.Interfaces;
using AppBlueprint.Contracts.Baseline.Webhook.Requests;
using AppBlueprint.Contracts.Baseline.Webhook.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/webhooks")]
[Produces("application/json")]
public class WebhookController : BaseController
{
    private readonly IWebhookRepository _webhookRepository;
    private readonly IWebhookDeliveryService _webhookDeliveryService;

    public WebhookController(
        IConfiguration configuration,
        IWebhookRepository webhookRepository,
        IWebhookDeliveryService webhookDeliveryService)
        : base(configuration)
    {
        _webhookRepository = webhookRepository ?? throw new ArgumentNullException(nameof(webhookRepository));
        _webhookDeliveryService = webhookDeliveryService ?? throw new ArgumentNullException(nameof(webhookDeliveryService));
    }

    /// <summary>
    /// Gets all webhook endpoints for the authenticated tenant.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WebhookResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WebhookResponse>>> GetAll(CancellationToken cancellationToken)
    {
        string tenantId = GetCurrentTenantId();
        IEnumerable<WebhookEntity> webhooks = await _webhookRepository.GetByTenantIdAsync(tenantId, cancellationToken);
        IEnumerable<WebhookResponse> response = webhooks.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Gets a webhook endpoint by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WebhookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WebhookResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        WebhookEntity? webhook = await _webhookRepository.GetByIdAsync(id, cancellationToken);
        if (webhook is null) return NotFound();

        string tenantId = GetCurrentTenantId();
        if (webhook.TenantId != tenantId) return NotFound();

        return Ok(MapToResponse(webhook));
    }

    /// <summary>
    /// Registers a new webhook endpoint.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WebhookResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WebhookResponse>> Create([FromBody] CreateWebhookRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        string tenantId = GetCurrentTenantId();

        var webhook = new WebhookEntity
        {
            Url = new Uri(request.Url),
            Secret = request.Secret,
            Description = request.Description,
            EventTypes = request.EventTypes,
            TenantId = tenantId
        };

        await _webhookRepository.AddAsync(webhook, cancellationToken);
        await _webhookRepository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = webhook.Id }, MapToResponse(webhook));
    }

    /// <summary>
    /// Updates an existing webhook endpoint.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateWebhookRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        WebhookEntity? webhook = await _webhookRepository.GetByIdAsync(id, cancellationToken);
        if (webhook is null) return NotFound();

        string tenantId = GetCurrentTenantId();
        if (webhook.TenantId != tenantId) return NotFound();

        webhook.Url = new Uri(request.Url);
        webhook.Secret = request.Secret;
        webhook.Description = request.Description;
        webhook.EventTypes = request.EventTypes;

        _webhookRepository.Update(webhook);
        await _webhookRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Deletes a webhook endpoint.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        WebhookEntity? webhook = await _webhookRepository.GetByIdAsync(id, cancellationToken);
        if (webhook is null) return NotFound();

        string tenantId = GetCurrentTenantId();
        if (webhook.TenantId != tenantId) return NotFound();

        await _webhookRepository.DeleteAsync(id, cancellationToken);
        await _webhookRepository.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static WebhookResponse MapToResponse(WebhookEntity webhook) => new()
    {
        Id = webhook.Id,
        Url = webhook.Url.ToString(),
        Description = webhook.Description,
        EventTypes = webhook.EventTypes,
        TenantId = webhook.TenantId
    };

    private string GetCurrentTenantId()
    {
        return User.FindFirst("tenant_id")?.Value ?? string.Empty;
    }
}
