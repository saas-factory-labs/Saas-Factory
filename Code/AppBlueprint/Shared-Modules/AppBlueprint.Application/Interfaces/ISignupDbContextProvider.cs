using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Database context provider for signup operations.
/// Provides access to DbContext for EF Core operations.
/// </summary>
public interface ISignupDbContextProvider
{
    /// <summary>
    /// Gets a database context for signup operations.
    /// </summary>
    Task<DbContext> GetDbContextAsync(CancellationToken cancellationToken = default);
}
