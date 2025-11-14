using AppBlueprint.Contracts.Baseline.Notification.Requests;
using AppBlueprint.Contracts.Baseline.Notification.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.SharedKernel;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/notifications")]
[Produces("application/json")]
public class NotificationController : BaseController
{
    private readonly ILogger<NotificationController> _logger;
    private readonly INotificationRepository _notificationRepository;

    public NotificationController(
        ILogger<NotificationController> logger,
        IConfiguration configuration,
        INotificationRepository notificationRepository)
        : base(configuration)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(notificationRepository);

        _logger = logger;
        _notificationRepository = notificationRepository;
    }

    /// <summary>
    /// Gets all notifications.
    /// </summary>
    /// <returns>List of notifications</returns>
    [HttpGet(ApiEndpoints.Notifications.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult<IEnumerable<NotificationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        IEnumerable<NotificationEntity> notifications = await _notificationRepository.GetAllAsync();
        if (!notifications.Any()) return NotFound(new { Message = "No notifications found." });

        IEnumerable<NotificationResponse> response = notifications.Select(notification => new NotificationResponse
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        });

        return Ok(response);
    }

    /// <summary>
    /// Gets a notification by ID.
    /// </summary>
    /// <param name="id">Notification ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Notification</returns>
    [HttpGet(ApiEndpoints.Notifications.GetById)]
    [ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult<NotificationResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        NotificationEntity? notification = await _notificationRepository.GetByIdAsync(id);
        if (notification is null) return NotFound(new { Message = $"Notification with ID {id} not found." });

        NotificationResponse response = new()
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a new notification.
    /// </summary>
    /// <param name="request">Notification creation request.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created notification.</returns>
    [HttpPost(ApiEndpoints.Notifications.Create)]
    [ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult<NotificationResponse>> Create([FromBody] CreateNotificationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        NotificationEntity newNotification = new()
        {
            Id = PrefixedUlid.Generate("notif"),
            OwnerId = "temp-owner-id", // TODO: Get from authenticated user context
            Title = request.Title,
            Message = request.Message,
            IsRead = request.IsRead,
            UserId = "temp-user-id", // TODO: Get from authenticated user context
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(newNotification);

        NotificationResponse response = new()
        {
            Id = newNotification.Id,
            Title = newNotification.Title,
            Message = newNotification.Message,
            CreatedAt = newNotification.CreatedAt,
            IsRead = newNotification.IsRead
        };

        return CreatedAtAction(nameof(GetById), new { id = newNotification.Id }, response);
    }

    /// <summary>
    /// Deletes a notification by ID.
    /// </summary>
    /// <param name="id">Notification ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpDelete(ApiEndpoints.Notifications.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        NotificationEntity? notification = await _notificationRepository.GetByIdAsync(id);
        if (notification is null) return NotFound(new { Message = $"Notification with ID {id} not found." });

        _notificationRepository.Delete(id);

        return NoContent();
    }
}
