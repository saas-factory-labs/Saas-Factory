using AppBlueprint.Application.Interfaces.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Implementation of <see cref="IConfigurationService"/> providing strongly-typed configuration access.
/// </summary>
public sealed class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public ConfigurationService(
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public T GetRequired<T>() where T : class, new()
    {
        IOptions<T>? options = _serviceProvider.GetService<IOptions<T>>();
        if (options?.Value is null)
        {
            throw new InvalidOperationException(
                $"Configuration for {typeof(T).Name} is not registered. Ensure AddAppBlueprintConfiguration() is called in Program.cs.");
        }
        return options.Value;
    }

    /// <inheritdoc />
    public T? GetOptional<T>() where T : class, new()
    {
        IOptions<T>? options = _serviceProvider.GetService<IOptions<T>>();
        return options?.Value;
    }

    /// <inheritdoc />
    public string? GetConnectionString(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return _configuration.GetConnectionString(name);
    }

    /// <inheritdoc />
    public string? GetSecret(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        // Always prefer environment variable for secrets (UPPERCASE_UNDERSCORE format)
        string envKey = key.Replace(":", "_", StringComparison.Ordinal);
        string? envValue = Environment.GetEnvironmentVariable(envKey);
        if (!string.IsNullOrEmpty(envValue))
        {
            return envValue;
        }

        // Fallback to configuration (could be from Key Vault or appsettings)
        return _configuration[key];
    }
}
