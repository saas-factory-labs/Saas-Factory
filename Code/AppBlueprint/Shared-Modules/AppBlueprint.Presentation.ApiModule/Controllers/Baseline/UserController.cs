using AppBlueprint.Contracts.Baseline.User.Requests;
using AppBlueprint.Contracts.Baseline.User.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
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
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    // Removed IUnitOfWork dependency for repository DI pattern
    private readonly IUserRepository _userRepository;

    public UserController(
        ILogger<AccountController> logger,
        IConfiguration configuration,
        IUserRepository userRepository)
        : base(configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        // Removed IUnitOfWork assignment
    }

    /// <summary>
    ///     Gets currently logged-in user profile.
    /// </summary>
    /// <returns>List of users</returns>
    /// Roles = Roles.RegisteredUser
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
    ///     Gets all users.
    /// </summary>
    /// <returns>List of users</returns>
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
    ///     Gets a user by ID.
    /// </summary>
    /// <param name="id">User ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>User</returns>
    [HttpGet(ApiEndpoints.Users.GetById)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> GetUserById(string id, CancellationToken cancellationToken)
    {
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
    ///     Creates a new user.
    /// </summary>
    /// <param name="createUser">User data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Created user.</returns>
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
            FirstName = createUser.Name,
            LastName = createUser.Name,
            UserName = createUser.Name,
            Email = createUser.Email,
            Profile = new ProfileEntity
            {
                // FirstName = createUser.Name,
                // LastName = createUser.Name,
                // UserName = createUser.Name,
                // Email = createUser.Email
            }
        };

        await _userRepository.AddAsync(newUser);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }

    /// <summary>
    ///     Updates an existing user.
    /// </summary>
    /// <param name="id">User ID.</param>
    /// <param name="createUser">User data transfer object.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpPut(ApiEndpoints.Users.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> UpdateUser(string id, [FromBody] CreateUserRequest createUser,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(createUser);

        UserEntity? existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser is null) return NotFound(new { Message = $"User with ID {id} not found." });

        existingUser.UserName = createUser.Name;
        existingUser.Email = createUser.Email;

        _userRepository.Update(existingUser);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }

    /// <summary>
    ///     Deletes a user by ID.
    /// </summary>
    /// <param name="id">User ID.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content.</returns>
    [HttpDelete(ApiEndpoints.Users.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [MapToApiVersion(ApiVersions.V1)]
    [MapToApiVersion(ApiVersions.V2)]
    public async Task<ActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        UserEntity? existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser is null) return NotFound(new { Message = $"User with ID {id} not found." });

        _userRepository.Delete(existingUser.Id);
        // If SaveChangesAsync is required, inject a service for it or handle in repository.

        return NoContent();
    }
}
