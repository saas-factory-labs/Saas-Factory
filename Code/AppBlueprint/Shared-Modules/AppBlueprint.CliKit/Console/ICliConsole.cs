namespace AppBlueprint.CliKit.Console;

public interface ICliConsole
{
    bool IsInteractive { get; }
    bool IsContinuousIntegration { get; }
    bool SupportsUnicode { get; }

    void ClearIfInteractive();
    void LogStep(string message, string? detail = null);
    void RenderError(string title, string message, string? details = null);
    Task<T> ExecuteWithStatusAsync<T>(string initialMessage, Func<Action<string>, Task<T>> work);
}
