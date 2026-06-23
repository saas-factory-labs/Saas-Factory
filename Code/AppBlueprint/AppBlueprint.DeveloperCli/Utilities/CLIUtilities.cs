using AppBlueprint.CliKit.Processes;
using Microsoft.Extensions.Logging.Abstractions;

namespace AppBlueprint.DeveloperCli.Utilities;

internal static class CliUtilities
{
    public static bool RunShellCommand(
        string command,
        string successMessage,
        string errorMessage,
        string workingDirectory = "")
    {
        try
        {
            AnsiConsole.MarkupLine($"[yellow]Running command: [bold]{command.EscapeMarkup()}[/][/]");

            var runner = new CliProcessRunner(NullLogger<CliProcessRunner>.Instance);
            var result = runner.RunAsync(new CliProcessRequest(
                    FileName: CliConfiguration.IsWindows ? "cmd.exe" : "/bin/sh",
                    Arguments: CliConfiguration.IsWindows ? $"/c {command}" : $"-c \"{command.Replace("\"", "\\\"")}\"",
                    WorkingDirectory: string.IsNullOrWhiteSpace(workingDirectory) ? null : workingDirectory,
                    Timeout: TimeSpan.FromSeconds(30),
                    StreamOutput: false))
                .GetAwaiter()
                .GetResult();

            if (!string.IsNullOrWhiteSpace(result.StandardOutput))
            {
                AnsiConsole.MarkupLine($"[blue]{result.StandardOutput.EscapeMarkup()}[/]");
            }

            if (!string.IsNullOrWhiteSpace(result.StandardError))
            {
                AnsiConsole.MarkupLine($"[red]{result.StandardError.EscapeMarkup()}[/]");
            }

            if (result.Succeeded)
            {
                AnsiConsole.MarkupLine($"[green]{successMessage.EscapeMarkup()}[/]");
                return true;
            }

            var suffix = result.TimedOut ? " (timed out)" : string.Empty;
            AnsiConsole.MarkupLine($"[red]{errorMessage.EscapeMarkup()}{suffix}[/]");
            return false;
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception or TimeoutException)
        {
            AnsiConsole.MarkupLine($"[red]Process error: {ex.Message.EscapeMarkup()}[/]");
            return false;
        }
    }
}
