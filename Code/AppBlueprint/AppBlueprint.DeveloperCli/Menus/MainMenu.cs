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
            "Scan API routes",
            "Clone a GitHub repository",
            "Generate JWT Token (for testing)",
            "Validate PostGreSQL Password",
            "Manage Environment Variable",
            "Exit"
        ];

        // Check if terminal supports interactive prompts
        if (!AnsiConsole.Profile.Capabilities.Interactive || !AnsiConsole.Profile.Capabilities.Ansi)
        {
            // Fallback to simple console menu for non-interactive terminals
            ShowSimpleMenu(options);
            return;
        }

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select an option:")
                .PageSize(10)
                .AddChoices(options));

        ProcessSelection(choice);
    }

    private static void ShowSimpleMenu(string[] options)
    {
        Console.WriteLine("\n=== Developer CLI Menu ===\n");
        
        for (int i = 0; i < options.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i]}");
        }
        
        Console.Write("\nPlease select an option (1-{0}): ", options.Length);
        
        if (int.TryParse(Console.ReadLine(), out int selection) && selection >= 1 && selection <= options.Length)
        {
            ProcessSelection(options[selection - 1]);
        }
        else
        {
            Console.WriteLine("Invalid selection. Please run the CLI again.");
        }
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
                DatabaseCommand.ExecuteInteractive();
                break;
            case "Scan API routes":
                AnsiConsole.MarkupLine("[yellow]Scanning API routes...[/]");
                RouteCommand.ExecuteInteractive();
                break;
            case "Clone a GitHub repository":
                AnsiConsole.MarkupLine("[yellow]Cloning GitHub repository...[/]");
                GitHubCommand.ExecuteInteractive();
                break;
            case "Generate JWT Token (for testing)":
                AnsiConsole.MarkupLine("[yellow]Generating JWT Token...[/]");
                JwtTokenCommand.ExecuteInteractive();
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
