using System;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Logging;

public static class PIILoggerExtensions
{
    /// <summary>
    /// Logs a message replacing potential PII with a reference ID.
    /// </summary>
    public static void LogSecure(this ILogger logger, LogLevel logLevel, string messageTemplate, string resourceId, Exception? exception = null)
    {
        // Example: logger.LogSecure(LogLevel.Error, "Error processing message {MessageId}", message.Id);
        // This ensures the actual message content (which might contain PII) is not logged.
        logger.Log(logLevel, exception, messageTemplate, resourceId);
    }

    /// <summary>
    /// Logs an error with a reference ID to avoid logging original message content.
    /// </summary>
    public static void LogSecureError(this ILogger logger, string messageTemplate, string resourceId, Exception? exception = null)
    {
        logger.LogSecure(LogLevel.Error, messageTemplate, resourceId, exception);
    }
}
