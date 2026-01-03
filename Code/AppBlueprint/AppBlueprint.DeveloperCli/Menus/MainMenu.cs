using AppBlueprint.DeveloperCli.Commands;
using AppBlueprint.DeveloperCli.Utilities;
using RazorConsole;

namespace AppBlueprint.DeveloperCli.Menus;

internal static class MainMenu
{
    public static void Show()
    {
        // Clear console for better presentation (safe clear that handles invalid console)
        TryClearConsole();

        // Display beautiful header using RazorConsole
        ShowHeader();

        string[] options =
        [
            "Start development environment (run)",
            "Display environment info (env:info)",
            "Run tests (test)",
            "Install CLI globally (saas command)",
            "Uninstall CLI from system",
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

        // Use RazorConsole for menu selection
        var prompt = new SelectionPrompt<string>()
            .Title("\n[bold yellow]⚡ What would you like to do?[/]")
            .PageSize(20)
            .HighlightStyle(new Style(foreground: Color.Cyan1, decoration: Decoration.Bold))
            .AddChoiceGroup("[bold green]🚀 Development[/]", [
                "Start development environment (run)",
                "Display environment info (env:info)",
                "Run tests (test)"
            ])
            .AddChoiceGroup("[bold blue]🔧 CLI Management[/]", [
                "Install CLI globally (saas command)",
                "Uninstall CLI from system"
            ])
            .AddChoiceGroup("[bold magenta]🏗️ Scaffolding[/]", [
                "Create a new SaaS app solution",
                "Create a new project in the solution",
                "Create a new item (e.g., API controller, DTO, Service)"
            ])
            .AddChoiceGroup("[bold yellow]💾 Database & Tools[/]", [
                "Migrate database",
                "Scan API routes",
                "Clone a GitHub repository"
            ])
            .AddChoiceGroup("[bold cyan]🔐 Authentication & Security[/]", [
                "Generate JWT Token (for testing)",
                "Validate PostGreSQL Password",
                "Manage Environment Variable"
            ])
            .AddChoices(["[bold white]❌ Exit[/]"]);

        try
        {
            string choice = AnsiConsole.Prompt(prompt);
            ProcessSelection(choice);
        }
        catch (NotSupportedException)
        {
            // Fallback to simple menu when ANSI is not supported
            ShowSimpleMenu(options);
        }
    }

    private static void ShowHeader()
    {
        try
        {
            // Create a fancy ASCII art title using RazorConsole + Spectre.Console
            AnsiConsole.Write(
                new FigletText("SaaS Factory")
                    .LeftJustified()
                    .Color(Color.Cyan1));

            // Add a nice subtitle panel
            AnsiConsole.Write(
                new Panel(
                    new Markup("[bold]Developer CLI[/] - [dim]Your development companion for building amazing SaaS[/]"))
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Cyan1)
                    .Padding(1, 0));

            AnsiConsole.WriteLine();
        }
        catch
        {
            // Fallback for non-ANSI terminals
            Console.WriteLine("\n=== SaaS Factory - Developer CLI ===\n");
        }
    }

    private static void TryClearConsole()
    {
        try
        {
            if (AnsiConsole.Profile.Capabilities.Interactive)
            {
                Console.Clear();
            }
        }
        catch (IOException)
        {
            // Console.Clear() fails when there's no valid console handle (e.g., in watch mode)
            // Ignore the error and continue without clearing
        }
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
        // Remove markup tags for matching
        string cleanChoice = choice.Replace("[bold green]", "", StringComparison.Ordinal)
            .Replace("[bold blue]", "", StringComparison.Ordinal)
            .Replace("[bold magenta]", "", StringComparison.Ordinal)
            .Replace("[bold yellow]", "", StringComparison.Ordinal)
            .Replace("[bold cyan]", "", StringComparison.Ordinal)
            .Replace("[bold white]", "", StringComparison.Ordinal)
            .Replace("[/]", "", StringComparison.Ordinal)
            .Replace("🚀 ", "", StringComparison.Ordinal)
            .Replace("🔧 ", "", StringComparison.Ordinal)
            .Replace("🏗️ ", "", StringComparison.Ordinal)
            .Replace("💾 ", "", StringComparison.Ordinal)
            .Replace("🔐 ", "", StringComparison.Ordinal)
            .Replace("❌ ", "", StringComparison.Ordinal);

        switch (cleanChoice)
        {
            case "Development":
            case "CLI Management":
            case "Scaffolding":
            case "Database & Tools":
            case "Authentication & Security":
                // These are group headers, do nothing
                return;
            case "Start development environment (run)":
                AnsiConsole.MarkupLine("[yellow]Starting development environment...[/]");
                RunCommand.ExecuteInteractive();
                break;
            case "Display environment info (env:info)":
                AnsiConsole.MarkupLine("[yellow]Displaying environment info...[/]");
                EnvironmentInfoCommand.ExecuteInteractive();
                break;
            case "Run tests (test)":
                AnsiConsole.MarkupLine("[yellow]Running tests...[/]");
                TestCommand.ExecuteInteractive();
                break;
            case "Install CLI globally (saas command)":
                AnsiConsole.MarkupLine("[yellow]Installing CLI globally...[/]");
                InstallCommand.ExecuteInteractive();
                break;
            case "Uninstall CLI from system":
                AnsiConsole.MarkupLine("[yellow]Uninstalling CLI...[/]");
                UninstallCommand.ExecuteInteractive();
                break;
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
                ValidatePostgreSqlPassword();
                break;
            case "Manage Environment Variable":
                AnsiConsole.MarkupLine("[yellow]Managing Environment Variable...[/]");
                EnvironmentVariableCommand.ExecuteInteractive();
                break;
            case "Exit":
                AnsiConsole.MarkupLine("[red]Goodbye![/]");
                break;
            default:
                AnsiConsole.MarkupLine("[red]Invalid option selected.[/]");
                break;
        }
    }

    private static void ValidatePostgreSqlPassword()
    {
        AnsiConsole.MarkupLine("[cyan]═══════════════════════════════════════════════════════[/]");
        AnsiConsole.MarkupLine("[bold cyan]PostgreSQL Connection Validator[/]");
        AnsiConsole.MarkupLine("[cyan]═══════════════════════════════════════════════════════[/]");
        AnsiConsole.WriteLine();

        // Check for existing environment variable
        string? envConnectionString = Environment.GetEnvironmentVariable("APPBLUEPRINT_DATABASE_CONNECTIONSTRING", EnvironmentVariableTarget.User);

        if (!string.IsNullOrWhiteSpace(envConnectionString))
        {
            AnsiConsole.MarkupLine("[green]✓[/] Found connection string in environment variable");

            if (AnsiConsole.Confirm("Test the stored connection string?", true))
            {
                TestConnectionString(envConnectionString);
                return;
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]⚠[/] No connection string found in environment variable [grey]APPBLUEPRINT_DATABASE_CONNECTIONSTRING[/]");
            AnsiConsole.WriteLine();
        }

        // Prompt for connection string
        AnsiConsole.MarkupLine("[yellow]Enter PostgreSQL connection details:[/]");
        AnsiConsole.WriteLine();

        string host = AnsiConsole.Ask<string>("Host (e.g., [grey]localhost[/]):", "localhost");
        string port = AnsiConsole.Ask<string>("Port:", "5432");
        string database = AnsiConsole.Ask<string>("Database name:", "appblueprintdb");
        string username = AnsiConsole.Ask<string>("Username:", "postgres");
        string password = AnsiConsole.Prompt(
            new TextPrompt<string>("Password:")
                .PromptStyle("yellow")
                .Secret());

        // Build connection string
        string connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

        AnsiConsole.WriteLine();
        TestConnectionString(connectionString);

        // Ask if user wants to save to environment variable
        if (AnsiConsole.Confirm("[yellow]Save this connection string to environment variable?[/]"))
        {
            try
            {
                Environment.SetEnvironmentVariable("APPBLUEPRINT_DATABASE_CONNECTIONSTRING", connectionString, EnvironmentVariableTarget.User);
                AnsiConsole.MarkupLine("[green]✓[/] Connection string saved to [cyan]APPBLUEPRINT_DATABASE_CONNECTIONSTRING[/]");
                AnsiConsole.MarkupLine("[grey]Note: You may need to restart your terminal for changes to take effect[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗[/] Failed to save environment variable: {ex.Message}");
            }
        }
    }

    private static void TestConnectionString(string connectionString)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("[yellow]Testing connection...[/]", ctx =>
            {
                try
                {
                    bool isValid = ConnectionStringValidator.ValidatePostgreSqlConnection(connectionString).GetAwaiter().GetResult();

                    if (isValid)
                    {
                        AnsiConsole.MarkupLine("[green]✓ Connection successful![/]");
                        AnsiConsole.WriteLine();

                        // Display masked connection string
                        string maskedConnectionString = MaskPassword(connectionString);
                        AnsiConsole.MarkupLine($"[grey]Connection string: {maskedConnectionString}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]✗ Connection failed[/]");
                        AnsiConsole.MarkupLine("[yellow]Please verify your connection details[/]");
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]✗ Connection failed: {ex.Message}[/]");
                }
            });

        AnsiConsole.WriteLine();
    }

    private static string MaskPassword(string connectionString)
    {
        var regex = new System.Text.RegularExpressions.Regex(
            @"Password=([^;]+)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return regex.Replace(connectionString, "Password=***");
    }
}
