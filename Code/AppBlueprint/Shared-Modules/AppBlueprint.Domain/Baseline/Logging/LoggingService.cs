namespace AppBlueprint.Domain.Baseline.Logging;

public sealed class LoggingService
{
    private LoggingService() { }

    // Placeholder for logging functionality
    // TODO: Implement structured logging methods

    public static Task LogInformationAsync(string message, object? context = null)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task LogWarningAsync(string message, object? context = null)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task LogErrorAsync(string message, Exception? exception = null, object? context = null)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task LogDebugAsync(string message, object? context = null)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
}
