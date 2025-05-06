using AppBlueprint.DeveloperCli.Commands;

namespace AppBlueprint.DeveloperCli.Menus;

internal static class MainMenu
{
    public static void Show()
    {
        string[] options =
        [
            "Create a new SaaS app solution",
            "Create a new project in the solution",
            "Create a new item (e.g., API controller, DTO, Service)",
            "Migrate database",
            "Clone a GitHub repository",
            "Validate PostGreSQL Password",
            "Manage Environment Variable",
            "Exit"
        ];

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select an option:")
                .PageSize(10)
                .AddChoices(options));

        ProcessSelection(choice);
    }

    private static void ProcessSelection(string choice)
    {
        switch (choice)
        {
            case "Create a new SaaS app solution":
                AnsiConsole.MarkupLine("[yellow]Creating new SaaS app solution...[/]");
                SolutionCommand.ExecuteInteractive();
                break;
            case "Create a new project in the solution":
                AnsiConsole.MarkupLine("[yellow]Creating new project in the solution...[/]");
                ProjectCommand.ExecuteInteractive();
                break;
            case "Create a new item (e.g., API controller, DTO, Service)":
                AnsiConsole.MarkupLine("[yellow]Creating new item...[/]");
                ItemCommand.ExecuteInteractive();
                break;
            case "Migrate database":
                AnsiConsole.MarkupLine("[yellow]Migrating database...[/]");
                Command command = DatabaseCommand.Create();

                command.Handler?.Invoke(
                    new InvocationContext(command.Parse()));
                break;

            case "Clone a GitHub repository":
                AnsiConsole.MarkupLine("[yellow]Cloning GitHub repository...[/]");
                GitHubCommand.ExecuteInteractive();
                break;
            case "Validate PostGreSQL Password":
                AnsiConsole.MarkupLine("[yellow]Validating PostGreSQL Password...[/]");
                // TODO: Implement password validation
                break;
            case "Manage Environment Variable":
                AnsiConsole.MarkupLine("[yellow]Managing Environment Variable...[/]");
                // TODO: Implement environment variable management
                break;
            case "Exit":
                AnsiConsole.MarkupLine("[red]Goodbye![/]");
                break;
            default:
                AnsiConsole.MarkupLine("[red]Invalid option selected.[/]");
                break;
        }
    }
}
