// using AppBlueprint.Infrastructure.Repositories.Interfaces;
// using AppBlueprint.Infrastructure.UnitOfWork;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
//
// namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
//
// [Authorize (Roles = Roles.RegisteredUser)]
// [ApiController]
// [Route("api/notifications")]
// [Produces("application/json")]
// public class NotificationsController : ControllerBase
// {
//     private readonly INotificationRepository _notificationRepository;
//     private readonly IUnitOfWork _unitOfWork;
//
//     public NotificationsController(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
//     {
//         _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
//         _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
//     }
//
//     /// <summary>
//     /// Gets all notifications.
//     /// </summary>
//     /// <returns>List of notifications</returns>
//     [HttpGet("GetNotifications")]
//     [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
//     public async Task<ActionResult<IEnumerable<NotificationDto>>> Get(CancellationToken cancellationToken)
//     {
//         var notifications = await _notificationRepository.GetAllAsync();
//         var response = notifications.Select(notification => new NotificationDto
//         {
//             Title = notification.Title,
//             Message = notification.Message
//         });
//         return Ok(response);
//     }
//
//     /// <summary>
//     /// Creates a new notification.
//     /// </summary>
//     /// <param name="notificationDto">Notification data transfer object.</param>
//     /// <returns>Created notification.</returns>
//     // [HttpPost]
//     // [ProducesResponseType(StatusCodes.Status201Created)]
//     // [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     // public async Task<ActionResult> Post([FromBody] NotificationDto notificationDto, CancellationToken cancellationToken)
//     // {
//     //     if (!ModelState.IsValid) return BadRequest(ModelState);
//     //
//     //     // var newNotification = new NotificationModel
//     //     // {
//     //     //     Title = notificationDto.Title,
//     //     //     Message = notificationDto.Message,
//     //     //     CreatedAt = DateTime.UtcNow
//     //     // };
//     //     //
//     //     // await _notificationRepository.AddAsync(newNotification, cancellationToken);
//     //     // await _unitOfWork.SaveChangesAsync(cancellationToken);
//     //
//     //     return CreatedAtAction(nameof(Get), new { id = newNotification.Id }, newNotification);
//     // }
//
//     /// <summary>
//     /// Deletes a notification by ID.
//     /// </summary>
//     /// <param name="id">Notification ID.</param>
//     /// <returns>No content.</returns>
//     [HttpDelete("GetNotification/{id}")]
//     [ProducesResponseType(StatusCodes.Status204NoContent)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
//     {
//         var notification = await _notificationRepository.GetByIdAsync(id);
//         if (notification is null) return NotFound(new { Message = $"Notification with ID {id} not found." });
//
//         // _notificationRepository.Delete(notification);
//         // await _unitOfWork.SaveChangesAsync(cancellationToken);
//
//         return NoContent();
//     }
// }
//
// public class NotificationDto
// {
//     public string Title { get; set; }
//     public string Message { get; set; }
// }

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
