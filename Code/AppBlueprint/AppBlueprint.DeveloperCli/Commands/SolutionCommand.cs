using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class SolutionCommand
{
    public static Command Create()
    {
        var nameOption = new Option<string>("--name", "The name of the solution.") { IsRequired = true };
        var createRepoOption = new Option<bool>("--create-repo", "Set to true to create a GitHub repository for the solution.");

        var command = new Command("create-solution",
            "Create a new SaaS app solution with an optional GitHub repository.")
        {
            nameOption,
            createRepoOption
        };

        command.SetHandler((string name, bool createRepo) =>
        {
            AnsiConsole.MarkupLine($"[green]Creating SaaS app solution: {name}...[/]");
            CliUtilities.RunShellCommand($"dotnet new saas-app-solution -o {name}", "Solution created successfully!",
                "Failed to create solution.");

            if (createRepo)
                CliUtilities.RunShellCommand($"gh repo create {name} --private --confirm",
                    "GitHub repository created successfully!", "Failed to create GitHub repository.");
        }, nameOption, createRepoOption);

        return command;
    }

    public static void ExecuteInteractive()
    {
        string name = AnsiConsole.Ask<string>("[green]Enter the solution name:[/]");
        bool createRepo = AnsiConsole.Confirm("[green]Do you want to create a GitHub repository?[/]");
        CliUtilities.RunShellCommand($"dotnet new saas-app-solution -o {name}", "Solution created successfully!",
            "Failed to create solution.");

        if (createRepo)
            CliUtilities.RunShellCommand($"gh repo create {name} --private --confirm",
                "GitHub repository created successfully!", "Failed to create GitHub repository.");
    }
}
