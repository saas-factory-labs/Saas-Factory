namespace AppBlueprint.Domain.Baseline.ApiLogging;

public static class ApiLoggingService
{
    public static Task LogRequestAsync(string endpoint, string method, string requestBody, string userId)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task LogResponseAsync(string endpoint, int statusCode, string responseBody, TimeSpan duration)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task LogErrorAsync(string endpoint, Exception exception, string userId)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
}
