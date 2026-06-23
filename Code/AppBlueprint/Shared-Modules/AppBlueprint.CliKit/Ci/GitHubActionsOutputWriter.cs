namespace AppBlueprint.CliKit.Ci;

public interface IGitHubActionsOutputWriter
{
    bool IsGitHubActions { get; }
    Task SetEnvironmentVariableAsync(string name, string value, CancellationToken cancellationToken = default);
    Task SetOutputAsync(string name, string value, CancellationToken cancellationToken = default);
}

public sealed class GitHubActionsOutputWriter : IGitHubActionsOutputWriter
{
    public bool IsGitHubActions =>
        string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), "true", StringComparison.OrdinalIgnoreCase);

    public Task SetEnvironmentVariableAsync(string name, string value, CancellationToken cancellationToken = default)
    {
        var file = Environment.GetEnvironmentVariable("GITHUB_ENV");
        return AppendFileCommandAsync(file, name, value, cancellationToken);
    }

    public Task SetOutputAsync(string name, string value, CancellationToken cancellationToken = default)
    {
        var file = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
        return AppendFileCommandAsync(file, name, value, cancellationToken);
    }

    private static async Task AppendFileCommandAsync(string? filePath, string name, string value, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        await File.AppendAllTextAsync(filePath, $"{name}={value}{Environment.NewLine}", cancellationToken);
    }
}
