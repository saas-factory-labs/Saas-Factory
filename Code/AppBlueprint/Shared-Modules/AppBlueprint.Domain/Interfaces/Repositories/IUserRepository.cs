using AppBlueprint.Domain.Entities.User;

namespace AppBlueprint.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entity operations
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">User unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User entity or null if not found.</returns>
    Task<UserEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">User email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User entity or null if not found.</returns>
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all users in the repository.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all user entities.</returns>
    Task<IEnumerable<UserEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user in the repository (legacy method, prefer AddAsync).
    /// </summary>
    /// <param name="user">User entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created user entity.</returns>
    Task<UserEntity> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">User entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Added user entity.</returns>
    Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="user">User entity with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated user entity.</returns>
    Task<UserEntity> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a user entity as modified (EF Core change tracking).
    /// </summary>
    /// <param name="user">User entity to mark as modified.</param>
    void Update(UserEntity user);

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">User unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists by their unique identifier.
    /// </summary>
    /// <param name="id">User unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if user exists, false otherwise.</returns>
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists by their email address.
    /// </summary>
    /// <param name="email">User email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if user exists, false otherwise.</returns>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
