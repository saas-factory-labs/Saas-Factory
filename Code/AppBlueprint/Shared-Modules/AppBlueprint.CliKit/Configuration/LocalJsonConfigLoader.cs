using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.CliKit.Configuration;

public sealed class LocalJsonConfigLoader<T> where T : class
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly ILogger<LocalJsonConfigLoader<T>> _logger;

    public LocalJsonConfigLoader(ILogger<LocalJsonConfigLoader<T>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T?> LoadAsync(string fileName, string? directory = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var searchDirectory = directory ?? Directory.GetCurrentDirectory();
        var configPath = Path.Combine(searchDirectory, fileName);

        if (!File.Exists(configPath))
        {
            _logger.LogDebug("No local JSON config found at {Path}; using defaults.", configPath);
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(configPath, cancellationToken);
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Local JSON config at {Path} contains invalid JSON; ignoring.", configPath);
            return null;
        }
        catch (IOException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Failed to read local JSON config at {Path}; ignoring.", configPath);
            return null;
        }
        catch (UnauthorizedAccessException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Failed to read local JSON config at {Path}; ignoring.", configPath);
            return null;
        }
        catch (NotSupportedException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Failed to read local JSON config at {Path}; ignoring.", configPath);
            return null;
        }
    }
}
