using AppBlueprint.Application.Services.Users;
using AppBlueprint.Contracts.Baseline.Auth.Requests;
using AppBlueprint.Domain.Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[ApiController]
[Route("api/v{version:apiVersion}/authentication")]
[Produces("application/json")]
public class AuthenticationController : BaseController
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        IUserService userService,
        IConfiguration configuration,
        ILogger<AuthenticationController> logger) : base(configuration)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <remarks>
    /// Authenticates a user and returns a JWT token if credentials are valid
    /// </remarks>
    [HttpPost(ApiEndpoints.Authentication.Login)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrEmpty(request.Email))
            return BadRequest(new { Message = "Email is required" });

        try
        {
            UserEntity? user = await _userService.GetByEmailAsync(request.Email, cancellationToken);

            // Guard clause: User not found
            if (user is null)
                return Unauthorized(new { Message = "Invalid credentials" });

            // Guard clause: Inactive account
            if (!user.IsActive)
                return Unauthorized(new { Message = "Account is deactivated" });

            // Happy path
            return Ok(new { Message = "Authentication successful" });
        }
        catch (InvalidOperationException)
        {
            return Unauthorized(new { Message = "Invalid credentials" });
        }
        catch (ArgumentException)
        {
            return Unauthorized(new { Message = "Invalid credentials" });
        }
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <remarks>
    /// Creates a new user account. Note that email verification is handled separately.
    /// </remarks>
    [HttpPost(ApiEndpoints.Authentication.Register)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrEmpty(request.Email))
            return BadRequest(new { Message = "Email is required" });

        try
        {
            UserEntity user = await _userService.RegisterAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.UserName,
                cancellationToken
            );

            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    /// <summary>
    /// Logs out a user
    /// </summary>
    /// <remarks>
    /// Logs out the currently authenticated user
    /// </remarks>
    [Authorize]
    [HttpPost(ApiEndpoints.Authentication.Logout)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        return Ok(new { Message = "Logout successful" });
    }

    /// <summary>
    /// Create user profile after successful authentication
    /// </summary>
    [Authorize]
    [HttpPost(ApiEndpoints.Authentication.CreateProfile)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            UserEntity user = await _userService.RegisterAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.UserName,
                cancellationToken
            );

            return CreatedAtAction(nameof(CreateProfile), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error during profile creation");
            return BadRequest(new { Message = "Unable to create profile. Please try again." });
        }
    }

    /// <summary>
    /// Deactivate user profile
    /// </summary>
    [Authorize]
    [HttpPost(ApiEndpoints.Authentication.DeactivateProfile)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateProfile(string userId, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.DeactivateUserAsync(userId, cancellationToken);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error during profile deactivation for user {UserId}", userId);
            return NotFound(new { Message = "User not found or unable to deactivate." });
        }
    }
}
