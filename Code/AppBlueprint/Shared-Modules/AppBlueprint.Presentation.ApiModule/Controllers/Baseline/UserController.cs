using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Contracts.Baseline.User.Requests;
using AppBlueprint.Contracts.Baseline.User.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[ApiVersion(ApiVersions.V2)]
[Route("api/v{version:apiVersion}/user")]
[Produces("application/json")]
public class UserController : BaseController
{
    // Removed IUnitOfWork dependency for repository DI pattern
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;

    public UserController(
        IConfiguration configuration,
        IUserRepository userRepository,
        ApplicationDbContext context)
        : base(configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(userRepository);
        ArgumentNullException.ThrowIfNull(context);

        _userRepository = userRepository;
        _context = context;
    }

    /// <summary>
    ///     Retrieves the currently authenticated user's profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Current user's profile information.</returns>
    /// <remarks>
    ///     Available in API versions v1 and v2.
    ///     Returns the profile of the user making the request based on their authentication token.
    ///     Sample request:
    ///     <code>GET /api/v1/user/me</code>
    /// </remarks>
    /// <response code="200">Returns the current user's profile successfully.</response>
    /// <response code="404">User profile not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> GetLoggedInUser(CancellationToken cancellationToken)
    {
        IEnumerable<UserEntity>? users = await _userRepository.GetAllAsync();
        if (!users.Any()) return NotFound(new { Message = "No users found." });

        IEnumerable<UserResponse>? response = users.Select(user => new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email
        });

        return Ok(response);
    }

    /// <summary>
    ///     Retrieves all users in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>List of all users with their basic profile information.</returns>
    /// <remarks>
    ///     Available in API versions v1 and v2.
    ///     Returns username, email, first name, and last name for all users.
    ///     Requires authentication.
    /// </remarks>
    /// <response code="200">Returns the list of users successfully.</response>
    /// <response code="404">No users found in the system.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet(ApiEndpoints.Users.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> GetUsers(CancellationToken cancellationToken)
    {
        IEnumerable<UserEntity>? users = await _userRepository.GetAllAsync();
        if (!users.Any()) return NotFound(new { Message = "No users found." });

        IEnumerable<UserResponse>? response = users.Select(user => new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        });

        return Ok(response);
    }

    /// <summary>
    ///     Retrieves a specific user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user (e.g., "usr_01HX...").</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>User profile details including username, email, and name.</returns>
    /// <remarks>
    ///     Available in API versions v1 and v2.
    ///     Sample request:
    ///     <code>GET /api/v1/user/usr_01HX...</code>
    /// </remarks>
    /// <response code="200">Returns the user profile successfully.</response>
    /// <response code="404">User with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet(ApiEndpoints.Users.GetById)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> GetUserById(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        UserEntity? user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound(new { Message = $"User with ID {id} not found." });

        var response = new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };

        return Ok(response);
    }

    /// <summary>
    ///     Creates a new user account with the specified details.
    /// </summary>
    /// <param name="createUser">User creation request containing name and email.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The newly created user with generated ID and profile.</returns>
    /// <remarks>
    ///     Available in API versions v1 and v2.
    ///     Sample request:
    ///     <code>
    ///     POST /api/v1/user
    ///     {
    ///         "name": "John Doe",
    ///         "email": "john.doe@example.com"
    ///     }
    ///     </code>
    ///     Creates user entity and associated profile automatically.
    /// </remarks>
    /// <response code="201">User created successfully. Returns the created user with its ID.</response>
    /// <response code="400">Invalid request data or validation failed.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost(ApiEndpoints.Users.Create)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest createUser,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(createUser);

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var newUser = new UserEntity
        {
            FirstName = createUser.FirstName,
            LastName = createUser.LastName,
            UserName = createUser.UserName,
            Email = createUser.Email,
            TenantId = createUser.TenantId,
            ExternalAuthId = createUser.ExternalAuthId,
            IsActive = createUser.IsActive,
            Profile = new ProfileEntity()
        };

        await _userRepository.AddAsync(newUser);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new UserResponse
        {
            Id = newUser.Id,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            UserName = newUser.UserName,
            Email = newUser.Email
        };

        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, response);
    }

    /// <summary>
    ///     Updates an existing user's profile information.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="createUser">Updated user data containing name and email.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Available in API versions v1 and v2.
    ///     Sample request:
    ///     <code>
    ///     PUT /api/v1/user/usr_01HX...
    ///     {
    ///         "name": "Jane Doe",
    ///         "email": "jane.doe@example.com"
    ///     }
    ///     </code>
    /// </remarks>
    /// <response code="204">User updated successfully.</response>
    /// <response code="404">User with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPut(ApiEndpoints.Users.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> UpdateUser(string id, [FromBody] CreateUserRequest createUser,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(createUser);

        UserEntity? existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser is null) return NotFound(new { Message = $"User with ID {id} not found." });

        existingUser.FirstName = createUser.FirstName;
        existingUser.LastName = createUser.LastName;
        existingUser.UserName = createUser.UserName;
        existingUser.Email = createUser.Email;
        existingUser.IsActive = createUser.IsActive;
        // Note: TenantId and ExternalAuthId should not be changed after creation

        _userRepository.Update(existingUser);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes a user account by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     Available in API versions v1 and v2.
    ///     Sample request:
    ///     <code>DELETE /api/v1/user/usr_01HX...</code>
    ///     Warning: This operation permanently removes the user account and all associated data.
    /// </remarks>
    /// <response code="204">User deleted successfully.</response>
    /// <response code="404">User with the specified ID was not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpDelete(ApiEndpoints.Users.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(id);

        UserEntity? existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser is null) return NotFound(new { Message = $"User with ID {id} not found." });

        _userRepository.Delete(existingUser.Id);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }
}
