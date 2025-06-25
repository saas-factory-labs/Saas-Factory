using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class GithubActionWorkflowCommand
{
    public static Command Create()
    {
        var saasNameOption = new Option<string>("--saas-name", "Name of the SaaS App") { IsRequired = true };
        
        var command = new Command("create-github-action-workflows", "Create github action workflows for a new SaaS app")
        {
            saasNameOption
        };

        command.SetHandler((string name) =>
        {
            AnsiConsole.MarkupLine(
                $"[green]Creating github action workflow files and pushing to Github remote repository: {name}...[/]");
            // run pulumi automation api code command to generate all neccesarry github action workflows for app project ..

            /* workflows that will be created:
             *
             * publish container image to github container package registry
             * publish nuget packages to github package registry
             * git guardian secret scanning
             * generate sbom file for dependency management
             * docker scout container vulnerability scan
             * build and test dotnet projects                 *
             * bytebase sql review
             * database migration workflow with guard rails to enable migration to production app database
             *
             */

            // CLIUtilities.RunShellCommand($" dotnet new saas-app-solution -o {name}", "Solution created successfully!", "Failed to create solution.");

            // if (createRepo)
            // {
            //     CLIUtilities.RunShellCommand($"gh repo create {name} --private --confirm", "GitHub repository created successfully!", "Failed to create GitHub repository.");
            // }
        }, saasNameOption);

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
