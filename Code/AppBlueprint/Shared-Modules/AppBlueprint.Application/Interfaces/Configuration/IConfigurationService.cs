namespace AppBlueprint.Application.Interfaces.Configuration;

/// <summary>
/// Provides strongly-typed access to application configuration.
/// Abstracts the underlying configuration source (appsettings, env vars, Key Vault, etc.).
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Gets configuration options of type T.
    /// Throws if configuration is invalid or missing.
    /// </summary>
    /// <typeparam name="T">The options type.</typeparam>
    /// <returns>The configuration options.</returns>
    /// <exception cref="InvalidOperationException">Thrown when configuration is not registered or invalid.</exception>
    T GetRequired<T>() where T : class, new();
    
    /// <summary>
    /// Gets configuration options of type T.
    /// Returns null if configuration section doesn't exist.
    /// </summary>
    /// <typeparam name="T">The options type.</typeparam>
    /// <returns>The configuration options or null.</returns>
    T? GetOptional<T>() where T : class, new();
    
    /// <summary>
    /// Gets a connection string by name.
    /// </summary>
    /// <param name="name">The connection string name.</param>
    /// <returns>The connection string or null if not found.</returns>
    string? GetConnectionString(string name);
    
    /// <summary>
    /// Gets a secret value (prioritizes environment variables over configuration).
    /// </summary>
    /// <param name="key">The configuration key (supports : or __ separators).</param>
    /// <returns>The secret value or null if not found.</returns>
    string? GetSecret(string key);
}
