using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class GitHubCommand
{
    public static Command Create()
    {
        var command = new Command("clone-repo", "Clone a GitHub repository.")
        {
            new Option<string>("--repo-url", "The URL of the GitHub repository."),
            new Option<string>("--output-dir", "The directory to clone the repository into.")
        };

        command.Handler = CommandHandler.Create<string, string>(
            (repoUrl, outputDir) =>
            {
                AnsiConsole.MarkupLine($"[green]Cloning repository {repoUrl} into {outputDir}...[/]");
                CliUtilities.RunShellCommand($"gh repo clone {repoUrl} {outputDir}", "Repository cloned successfully!",
                    "Failed to clone repository.");
            });

        return command;
    }

    public static void ExecuteInteractive()
    {
        string repoUrl = AnsiConsole.Ask<string>("[green]Enter the GitHub repository URL:[/]");
        string outputDir = AnsiConsole.Ask<string>("[green]Enter the output directory for the clone:[/]");
        CliUtilities.RunShellCommand($"gh repo clone {repoUrl} {outputDir}", "Repository cloned successfully!",
            "Failed to clone repository.");
    }
}
