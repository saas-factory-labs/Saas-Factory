using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class ProjectCommand
{
    public static Command Create()
    {
        var command = new Command("create-project", "Create a new project in the SaaS app solution.")
        {
            new Option<string>("--name", "The name of the project."),
            new Option<string>("--template", "The project template to use.")
        };

        command.Handler = CommandHandler.Create<string, string>(
            (name, template) =>
            {
                AnsiConsole.MarkupLine($"[green]Creating project: {name} using template {template}...[/]");
                CliUtilities.RunShellCommand($"dotnet new {template} -o {name}", "Project created successfully!",
                    "Failed to create project.");
            });

        return command;
    }

    public static void ExecuteInteractive()
    {
        string name = AnsiConsole.Ask<string>("[green]Enter the project name:[/]");
        string template = AnsiConsole.Ask<string>("[green]Enter the template to use (e.g., api, classlib):[/]");
        CliUtilities.RunShellCommand($"dotnet new {template} -o {name}", "Project created successfully!",
            "Failed to create project.");
    }
}
