using Spectre.Console;

namespace AppBlueprint.CliKit.Console;

public sealed class SpectreCliConsole : ICliConsole
{
    private const string AccentColor = "deepskyblue1";
    private const string DetailColor = "grey58";

    public bool IsInteractive =>
        !global::System.Console.IsOutputRedirected &&
        !global::System.Console.IsErrorRedirected;

    public bool IsContinuousIntegration =>
        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CI")) ||
        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

    public bool SupportsUnicode => AnsiConsole.Profile.Capabilities.Unicode && !IsContinuousIntegration;

    public void ClearIfInteractive()
    {
        if (!IsInteractive)
        {
            return;
        }

        try
        {
            AnsiConsole.Clear();
        }
        catch (IOException)
        {
            // Some hosts expose a console handle but still reject clear/cursor APIs.
        }
    }

    public void LogStep(string message, string? detail = null)
    {
        var prefix = SupportsUnicode ? ">" : "-";
        AnsiConsole.MarkupLine(
            $"[{AccentColor}]{Markup.Escape(prefix)}[/] [white]{Markup.Escape(message)}[/]" +
            (detail is null ? string.Empty : $" [{DetailColor}]({Markup.Escape(detail)})[/]"));
    }

    public void RenderError(string title, string message, string? details = null)
    {
        var content = new Markup(
            $"[red]{Markup.Escape(message)}[/]" +
            (details is null ? string.Empty : $"{Environment.NewLine}{Environment.NewLine}[{DetailColor}]{Markup.Escape(details)}[/]"));

        AnsiConsole.Write(
            new Panel(content)
                .Header($"[bold red] {Markup.Escape(title)} [/]", Justify.Left)
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("red dim")));
    }

    public async Task<T> ExecuteWithStatusAsync<T>(string initialMessage, Func<Action<string>, Task<T>> work)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(initialMessage);
        ArgumentNullException.ThrowIfNull(work);

        if (!IsInteractive || IsContinuousIntegration)
        {
            LogStep(initialMessage);
            return await work(message => LogStep(message));
        }

        return await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse($"{AccentColor} bold"))
            .StartAsync($"[{DetailColor}]{Markup.Escape(initialMessage)}[/]", async ctx =>
            {
                void UpdateStatus(string message) => ctx.Status($"[{DetailColor}]{Markup.Escape(message)}[/]");
                return await work(UpdateStatus);
            });
    }
}
